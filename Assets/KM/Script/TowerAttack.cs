using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Script.SO;

public class TowerAttack : MonoBehaviour
{
    [Header("SO & 설정")]
    [SerializeField] private ThrowObjectSO _objectSO;
    [SerializeField] private GameObject _baseObj;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private List<GameObject> RayDir = new();
    public AttakTowerSetting attakTower;
    private TowerSetting attakTowerSetting;

    [Header("상태")]
    public bool IsAlreadySynergy = false;
    private float fireSpeed;
    private float count;
    private string nameObj;
    private bool isCanAttack = true;
    private AudioSource fire;

    [Header("투사체")]
    public Stack<GameObject> throws = new();

    [Header("실행 중 시너지 상태 (읽기 전용)")]
    [SerializeField] private List<SynergyGroup> runtimeSynergies = new();

    // 시너지 추적
    private Dictionary<TowerAttack, (int groupIndex, int synergyIndex)> givenSynergies = new();
    private (TowerAttack from, int groupIndex, int synergyIndex)? receivedSynergy = null;

    private void Awake()
    {
        fire = GetComponent<AudioSource>();
        attakTowerSetting = GetComponent<TowerSetting>();
        attakTower = attakTowerSetting._attakTowerSetting;
        fireSpeed = _objectSO.Speed;
        count = _objectSO.ThrowCount;
        nameObj = _objectSO.Name;

        for (int i = 0; i < 10; i++)
        {
            GameObject throwObj = Instantiate(_baseObj, transform);
            throwObj.name = nameObj;
            throwObj.SetActive(false);
            throwObj.GetComponent<SpriteRenderer>().sprite = _objectSO.Sprite;
            throws.Push(throwObj);
        }

        // SO 시너지 복사
        runtimeSynergies = attakTower.Synergies
            .Select(group => new SynergyGroup
            {
                SynergyName = group.SynergyName,
                selectedSynergies = new List<Synergy>(group.selectedSynergies)
            }).ToList();
    }

    private void FixedUpdate()
    {
        ProcessSynergy();

        Collider2D hits = Physics2D.OverlapCircle(transform.position, attakTowerSetting.attackDistance, _targetMask);
        if (hits && isCanAttack)
        {
            isCanAttack = false;
            StartCoroutine(Attacking(hits));
        }
    }

    public void ProcessSynergy()
    {
        foreach (var direction in RayDir)
        {
            if (direction == null) continue;

            Vector2 origin = transform.position;
            Vector2 target = direction.transform.position;
            Vector2 dir = (target - origin).normalized;
            float dist = Vector2.Distance(origin, target);
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, towerLayer);
            Debug.DrawRay(origin, dir * dist, Color.yellow, 0.2f);

            TowerAttack hitTowerAttack = hit.collider?.GetComponent<TowerAttack>();
            TowerSetting hitTowerSetting = hit.collider?.GetComponent<TowerSetting>();

            if (hitTowerAttack == null || hitTowerSetting == null) continue;
            if (givenSynergies.ContainsKey(hitTowerAttack)) continue;
            if (hitTowerAttack.IsAlreadySynergy) continue;
            Debug.Log(hit.collider.gameObject.name);
            for (int g = 0; g < runtimeSynergies.Count; g++)
            {
                var synergyGroup = runtimeSynergies[g];
                var synergyList = synergyGroup.selectedSynergies;

                if (synergyList.All(s => s == Synergy.Ready)) continue;

                int matchIndex = synergyList.FindIndex(s => s != Synergy.Ready && s == hitTowerSetting.TowerType);
                if (matchIndex != -1)
                {
                    synergyList[matchIndex] = Synergy.Ready;

                    Debug.Log($"[Synergy Added] '{synergyGroup.SynergyName}'에 '{hitTowerSetting.TowerType}' 추가됨");

                    if (synergyList.All(s => s == Synergy.Ready))
                    {
                        Debug.Log($"[Synergy Complete] {synergyGroup.SynergyName} 완성!! 🎉");
                    }

                    givenSynergies[hitTowerAttack] = (g, matchIndex);
                    hitTowerAttack.receivedSynergy = (this, g, matchIndex);
                    hitTowerAttack.IsAlreadySynergy = true;
                    break;
                }
            }
        }
    }

    private IEnumerator Attacking(Collider2D collider)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject throwObj = throws.Pop();
            throwObj.SetActive(true);
            throwObj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            throwObj.transform.position = transform.position;
            throwObj.transform.rotation = Quaternion.LookRotation(collider.transform.position - transform.position) * Quaternion.Euler(0, 90, 90);
            throwObj.GetComponent<Rigidbody2D>().AddForce((Vector2)(collider.transform.position - transform.position) * fireSpeed, ForceMode2D.Impulse);
            throwObj.GetComponent<AudioSource>().clip = _objectSO.Audio;
            _objectSO.HitEffect.GetComponent<AudioSource>().clip = _objectSO.Audio;
            throwObj.GetComponent<ThrowScript>().hitEffect = _objectSO.HitEffect;
            fire.Play();
            yield return new WaitForSeconds(i <= count ? attakTowerSetting.attackDelay / 2 : attakTowerSetting.attackDelay);
        }
        isCanAttack = true;
    }

    private void OnDisable()
    {
        // 시너지 제거: 내가 준 경우
        foreach (var kvp in givenSynergies)
        {
            TowerAttack target = kvp.Key;
            var (groupIdx, synergyIdx) = kvp.Value;

            if (target != null && target.runtimeSynergies.Count > groupIdx)
            {
                var group = target.runtimeSynergies[groupIdx];
                group.selectedSynergies[synergyIdx] = target.attakTower.synergy;

                target.IsAlreadySynergy = false;
                target.receivedSynergy = null;

                Debug.Log($"[Synergy Removed] '{target.name}' → '{group.SynergyName}' 해제됨");
            }
        }

        // 시너지 제거: 내가 받은 경우
        if (receivedSynergy.HasValue)
        {
            var (from, groupIdx, synergyIdx) = receivedSynergy.Value;
            if (from != null && from.runtimeSynergies.Count > groupIdx)
            {
                var group = from.runtimeSynergies[groupIdx];
                group.selectedSynergies[synergyIdx] = from.attakTower.synergy;
                from.givenSynergies.Remove(this);

                Debug.Log($"[Synergy Removed] '{name}' 제거로 '{from.name}' 조합 해제");
            }
        }

        givenSynergies.Clear();
        receivedSynergy = null;
    }

    [ContextMenu("Synergy/Reset")]
    public void ResetSynergies()
    {
        IsAlreadySynergy = false;
        givenSynergies.Clear();
        receivedSynergy = null;

        foreach (var group in runtimeSynergies)
        {
            for (int i = 0; i < group.selectedSynergies.Count; i++)
            {
                if (group.selectedSynergies[i] == Synergy.Ready)
                    group.selectedSynergies[i] = attakTower.synergy;
            }
        }

        Debug.Log($"[Synergy Reset] {name} 초기화 완료");
    }

    [ContextMenu("Synergy/Log Current State")]
    public void LogSynergyState()
    {
        foreach (var group in runtimeSynergies)
        {
            string status = string.Join(", ", group.selectedSynergies.Select(s => s.ToString()));
            Debug.Log($"[Synergy Status] {group.SynergyName} : {status}");
        }
    }

    [ContextMenu("Synergy/Test Add Dummy")]
    public void ForceSynergyTest()
    {
        foreach (var group in runtimeSynergies)
        {
            for (int i = 0; i < group.selectedSynergies.Count; i++)
            {
                if (group.selectedSynergies[i] != Synergy.Ready)
                {
                    group.selectedSynergies[i] = Synergy.Ready;
                    Debug.Log($"[Synergy Dummy] {group.SynergyName} 항목 {i} 강제 Ready");
                    break;
                }
            }
        }
    }
}
