using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Script.SO;

public class TowerAttack : MonoBehaviour
{
    public static List<TowerAttack> AllTowers = new();

    [SerializeField] private ThrowObjectSO _objectSO;
    [SerializeField] private GameObject _baseObj;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask towerLayer;
    public AttakTowerSetting attakTower;
    private TowerSetting attakTowerSetting;

    public bool IsAlreadySynergy = false;
    public bool CanBeUsedAsIngredient = true;
    private float fireSpeed;
    private float count;
    private string nameObj;
    private bool isCanAttack = true;
    private AudioSource fire;

    public Stack<GameObject> throws = new();

    [SerializeField] private List<SynergyGroup> runtimeSynergies = new();

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

        runtimeSynergies = attakTower.Synergies
            .Select(group => new SynergyGroup
            {
                SynergyName = group.SynergyName,
                selectedSynergies = new List<Synergy>(group.selectedSynergies),
                IsCompleted = false
            }).ToList();

        TryApplySelfSynergy();
    }

    private void OnEnable() => AllTowers.Add(this);

    private void OnDisable()
    {
        AllTowers.Remove(this);

        ResetGivenSynergies();

        if (receivedSynergy.HasValue)
        {
            var (from, groupIdx, synergyIdx) = receivedSynergy.Value;
            if (from != null && from.runtimeSynergies.Count > groupIdx)
            {
                var group = from.runtimeSynergies[groupIdx];
                group.selectedSynergies[synergyIdx] = from.attakTower.synergy;
                group.IsCompleted = false;
                from.givenSynergies.Remove(this);
                from.IsAlreadySynergy = false;
            }
        }

        receivedSynergy = null;
        IsAlreadySynergy = false;
        CanBeUsedAsIngredient = true;
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
        foreach (var other in AllTowers)
        {
            if (other == this) continue;
            if (givenSynergies.ContainsKey(other)) continue;
            if (!other.CanBeUsedAsIngredient) continue;
            if (!IsInSight(other)) continue;

            for (int g = 0; g < runtimeSynergies.Count; g++)
            {
                var synergyGroup = runtimeSynergies[g];
                var synergyList = synergyGroup.selectedSynergies;

                if (synergyList.All(s => s == Synergy.Ready)) continue;

                int matchIndex = synergyList.FindIndex(s => s != Synergy.Ready && s == other.attakTower.synergy);

                if (matchIndex != -1)
                {
                    synergyList[matchIndex] = Synergy.Ready;

                    givenSynergies[other] = (g, matchIndex);
                    other.receivedSynergy = (this, g, matchIndex);
                    Debug.Log("AAAAAA");
                    if (synergyList.All(s => s == Synergy.Ready) && !synergyGroup.IsCompleted)
                    {
                        synergyGroup.IsCompleted = true;
                        IsAlreadySynergy = true;
                        Debug.Log(synergyGroup.SynergyName);
                    }

                    return;
                }
            }
        }
    }

    private void TryApplySelfSynergy()
    {
        for (int g = 0; g < runtimeSynergies.Count; g++)
        {
            var group = runtimeSynergies[g];
            var synergyList = group.selectedSynergies;

            bool appliedSelf = false;
            for (int i = 0; i < synergyList.Count; i++)
            {
                if (synergyList[i] != Synergy.Ready && synergyList[i] == attakTower.synergy && !appliedSelf)
                {
                    synergyList[i] = Synergy.Ready;
                    appliedSelf = true;
                }
            }

            if (synergyList.All(s => s == Synergy.Ready) && !group.IsCompleted)
            {
                group.IsCompleted = true;
                IsAlreadySynergy = true;
            }
        }
    }

    private bool IsInSight(TowerAttack target)
    {
        Vector2 origin = transform.position;
        Vector2 targetPos = target.transform.position;
        float dist = Vector2.Distance(origin, targetPos);

        if (dist > attakTowerSetting.attackDistance)
            return false;

        Vector2 dir = (targetPos - origin).normalized;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, towerLayer);
        Debug.DrawRay(origin, dir * dist, Color.yellow, 0.2f);
        return hit && hit.collider.gameObject == target.gameObject;
    }

    private void ResetGivenSynergies()
    {
        var toRemove = new List<TowerAttack>(givenSynergies.Keys);
        foreach (var target in toRemove)
        {
            if (target == null) continue;

            var (groupIdx, synergyIdx) = givenSynergies[target];
            if (target.runtimeSynergies.Count > groupIdx)
            {
                var group = target.runtimeSynergies[groupIdx];
                group.selectedSynergies[synergyIdx] = target.attakTower.synergy;
                group.IsCompleted = false;

                target.IsAlreadySynergy = false;
                target.receivedSynergy = null;
            }

            givenSynergies.Remove(target);
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

    [ContextMenu("Synergy/Reset")]
    public void ResetSynergies()
    {
        IsAlreadySynergy = false;
        CanBeUsedAsIngredient = true;

        if (receivedSynergy.HasValue)
        {
            var (from, groupIdx, synergyIdx) = receivedSynergy.Value;
            if (from != null && from.runtimeSynergies.Count > groupIdx)
            {
                var group = from.runtimeSynergies[groupIdx];
                group.selectedSynergies[synergyIdx] = from.attakTower.synergy;
                group.IsCompleted = false;
                from.givenSynergies.Remove(this);
                from.IsAlreadySynergy = false;
            }
        }

        ResetGivenSynergies();
        receivedSynergy = null;

        foreach (var group in runtimeSynergies)
        {
            for (int i = 0; i < group.selectedSynergies.Count; i++)
            {
                if (group.selectedSynergies[i] == Synergy.Ready)
                    group.selectedSynergies[i] = attakTower.synergy;
            }

            group.IsCompleted = false;
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
                    break;
                }
            }
        }
    }
}
