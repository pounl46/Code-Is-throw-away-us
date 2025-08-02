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
    public float damage;
    public float BulletSpeed;
    public bool isSlow;

    public Stack<GameObject> throws = new();

    [SerializeField] private List<SynergyGroup> runtimeSynergies = new();
    private List<SynergyGroup> originSynergies = new();
    private Dictionary<TowerAttack, List<(int groupIndex, int synergyIndex)>> givenSynergies = new();
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
        runtimeSynergies = attakTower.Synergies.Select(g => g.Clone()).ToList();
        originSynergies = attakTower.Synergies.Select(g => g.Clone()).ToList();

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
                var origin = from.originSynergies[groupIdx];
                group.selectedSynergies[synergyIdx] = origin.selectedSynergies[synergyIdx];
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
            if (other == this || !other.CanBeUsedAsIngredient || !IsInSight(other))
                continue;

            if (AllTowers.IndexOf(other) > AllTowers.IndexOf(this))
                continue;

            for (int g = 0; g < runtimeSynergies.Count; g++)
            {
                var group = runtimeSynergies[g];
                for (int i = 0; i < group.selectedSynergies.Count; i++)
                {
                    if (group.selectedSynergies[i] != Synergy.Ready &&
                        group.selectedSynergies[i] == other.attakTower.synergy)
                    {
                        group.selectedSynergies[i] = Synergy.Ready;

                        if (!givenSynergies.ContainsKey(other))
                            givenSynergies[other] = new();

                        if (!givenSynergies[other].Contains((g, i)))
                            givenSynergies[other].Add((g, i));

                        other.receivedSynergy = (this, g, i);

                        bool wasCompleted = group.IsCompleted;
                        group.IsCompleted = group.selectedSynergies.All(s => s == Synergy.Ready);
                        if (group.IsCompleted && !wasCompleted)
                        {
                            IsAlreadySynergy = true;
                            ApplySynergyEffect(group);
                        }
                    }
                    else if (group.selectedSynergies[i] == Synergy.Ready &&
                             group.selectedSynergies[i] != other.attakTower.synergy)
                    {
                        if (group.IsCompleted)
                            RemoveSynergyEffect(group);

                        var origin = originSynergies[g];
                        group.selectedSynergies[i] = origin.selectedSynergies[i];
                        group.IsCompleted = false;
                        IsAlreadySynergy = false;
                    }
                }
            }

            for (int g = 0; g < other.runtimeSynergies.Count; g++)
            {
                var group = other.runtimeSynergies[g];
                for (int i = 0; i < group.selectedSynergies.Count; i++)
                {
                    if (group.selectedSynergies[i] != Synergy.Ready &&
                        group.selectedSynergies[i] == attakTower.synergy)
                    {
                        group.selectedSynergies[i] = Synergy.Ready;

                        if (!other.givenSynergies.ContainsKey(this))
                            other.givenSynergies[this] = new();

                        if (!other.givenSynergies[this].Contains((g, i)))
                            other.givenSynergies[this].Add((g, i));

                        other.receivedSynergy = (this, g, i);

                        // 완성 체크
                        bool wasCompleted = group.IsCompleted;
                        group.IsCompleted = group.selectedSynergies.All(s => s == Synergy.Ready);
                        if (group.IsCompleted && !wasCompleted)
                        {
                            other.IsAlreadySynergy = true;
                            other.ApplySynergyEffect(group);
                        }
                    }
                    else if (group.selectedSynergies[i] == Synergy.Ready &&
                             group.selectedSynergies[i] != attakTower.synergy)
                    {
                        if (group.IsCompleted)
                            other.RemoveSynergyEffect(group);

                        var origin = other.originSynergies[g];
                        group.selectedSynergies[i] = origin.selectedSynergies[i];
                        group.IsCompleted = false;
                        other.IsAlreadySynergy = false;
                    }
                }
            }
        }
    }

    private void TryApplySelfSynergy()
    {
        for (int g = 0; g < runtimeSynergies.Count; g++)
        {
            var group = runtimeSynergies[g];
            bool appliedSelf = false;

            for (int i = 0; i < group.selectedSynergies.Count; i++)
            {
                if (group.selectedSynergies[i] != Synergy.Ready && group.selectedSynergies[i] == attakTower.synergy && !appliedSelf)
                {
                    group.selectedSynergies[i] = Synergy.Ready;
                    appliedSelf = true;
                }
            }

            if (group.selectedSynergies.All(s => s == Synergy.Ready) && !group.IsCompleted)
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

            foreach (var (groupIdx, synergyIdx) in givenSynergies[target])
            {
                if (target.runtimeSynergies.Count > groupIdx)
                {
                    var group = target.runtimeSynergies[groupIdx];
                    var origin = target.originSynergies[groupIdx];
                    group.selectedSynergies[synergyIdx] = origin.selectedSynergies[synergyIdx];
                    group.IsCompleted = false;
                    target.IsAlreadySynergy = false;
                    target.receivedSynergy = null;
                }
            }

            givenSynergies[target].Clear();
        }

        givenSynergies.Clear();
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
                var origin = from.originSynergies[groupIdx];
                group.selectedSynergies[synergyIdx] = origin.selectedSynergies[synergyIdx];
                group.IsCompleted = false;
                from.givenSynergies.Remove(this);
                from.IsAlreadySynergy = false;
            }
        }

        ResetGivenSynergies();
        receivedSynergy = null;

        for (int g = 0; g < runtimeSynergies.Count; g++)
        {
            var group = runtimeSynergies[g];
            var originGroup = originSynergies[g];

            for (int i = 0; i < group.selectedSynergies.Count; i++)
            {
                group.selectedSynergies[i] = originGroup.selectedSynergies[i];
            }

            group.IsCompleted = false;
        }
    }

    [ContextMenu("Synergy/ForceTest")]
    public void ForceSynergyTest()
    {
        foreach (var group in runtimeSynergies)
        {
            for (int i = 0; i < group.selectedSynergies.Count; i++)
            {
                group.selectedSynergies[i] = Synergy.Ready;
            }

            group.IsCompleted = true;
        }

        IsAlreadySynergy = true;
    }
    private void ApplySynergyEffect(SynergyGroup group)
    {
        damage *= group.ChangeDamage > 0 ? group.ChangeDamage : 1f;
        attakTowerSetting.Health *= group.ChangeHealth > 0 ? group.ChangeHealth : 1f;
        BulletSpeed *= group.BulletSpeed > 0 ? group.BulletSpeed : 1f;
        attakTowerSetting.attackDelay *= group.AttacingSpeed > 0 ? group.ChangeHealth : 1f;
        isSlow = group.Slow;
        Debug.Log(group.SynergyName);
    }
    
    private void RemoveSynergyEffect(SynergyGroup group)
    {
        damage = attakTowerSetting._attakTowerSetting.AttackDamage;
        attakTowerSetting.Health = attakTowerSetting._attakTowerSetting.TowerHealth;
        BulletSpeed = 1f;
        if (attakTowerSetting.TowerType == Synergy.Poison) isSlow = true;
        else isSlow = false;
        attakTowerSetting.attackDelay = attakTowerSetting._attakTowerSetting.AttackDelay;
        Debug.Log(group.SynergyName);
    }
}