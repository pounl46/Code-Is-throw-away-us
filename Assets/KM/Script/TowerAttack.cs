using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Script.SO;

public class TowerAttack : MonoBehaviour
{
    public static List<GameObject> AllTowers = new List<GameObject>();

    [SerializeField] private ThrowObjectSO _objectSO;
    [SerializeField] private GameObject _baseObj;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask towerLayer;
    public AttakTowerSetting attakTower;
    private TowerSetting attakTowerSetting;
    [SerializeField] private List<SynergyGroup> mySynergyGroups = new();
    private float fireSpeed;
    private float count;
    private string nameObj;
    private bool isCanAttack = true;
    private AudioSource fire;
    public float damage;
    public float BulletSpeed;
    public bool isSlow;

    public bool isSynergyAlready = false;

    public Stack<GameObject> throws = new();

    private void Awake()
    {
        fire = GetComponent<AudioSource>();
        attakTowerSetting = GetComponent<TowerSetting>();
        attakTower = attakTowerSetting._attakTowerSetting;

        fireSpeed = _objectSO.Speed;
        count = _objectSO.ThrowCount;
        nameObj = _objectSO.Name;
        AllTowers.Add(gameObject);

        mySynergyGroups = DeepCopySynergyGroups(attakTower.Synergies);

        for (int i = 0; i < 10; i++)
        {
            GameObject throwObj = Instantiate(_baseObj, transform);
            throwObj.name = nameObj;
            throwObj.SetActive(false);
            throwObj.GetComponent<SpriteRenderer>().sprite = _objectSO.Sprite;
            throws.Push(throwObj);
        }
    }

    private void OnDisable() => AllTowers.Remove(gameObject);

    private void OnDestroy()
    {
        attakTowerSetting._attakTowerSetting.ResetToInitialState();
    }

    private void FixedUpdate()
    {
        Collider2D hits = Physics2D.OverlapCircle(transform.position, attakTowerSetting.attackDistance, _targetMask);
        if (hits && isCanAttack)
        {
            isCanAttack = false;
            StartCoroutine(Attacking(hits));
        }

    //     foreach (var other in AllTowers)
    //     {
    //         if (IsInSight(other))
    //         {
    //             TowerSetting towerSetOther = other.GetComponent<TowerSetting>();
    //             TowerAttack otherAttack = other.GetComponent<TowerAttack>();

    //             foreach (var group in mySynergyGroups)
    //             {
    //                 if (group.IsCompleted) continue;

    //                 bool containsMyType = group.selectedSynergies.Contains(attakTowerSetting.TowerType);
    //                 bool containsOtherType = group.selectedSynergies.Contains(towerSetOther.TowerType);

    //                 if (containsMyType && containsOtherType)
    //                 {
    //                     for (int i = 0; i < group.selectedSynergies.Count; i++)
    //                     {
    //                         if (group.selectedSynergies[i] == attakTowerSetting.TowerType)
    //                         {
    //                             group.selectedSynergies[i] = Synergy.Ready;
    //                             attakTowerSetting.TowerType = Synergy.Ready;
    //                         }
    //                         else if (group.selectedSynergies[i] == towerSetOther.TowerType)
    //                         {
    //                             group.selectedSynergies[i] = Synergy.Ready;
    //                             towerSetOther.TowerType = Synergy.Ready;
    //                         }
    //                     }

    //                     CheckSynergyComplete(mySynergyGroups);
    //                 }
    //             }
    //         }
    //     }
    }

    // private void CheckSynergyComplete(List<SynergyGroup> synergyGroups)
    // {
    //     foreach (var group in synergyGroups)
    //     {
    //         if (group.IsCompleted) continue;

    //         bool allReady = true;

    //         foreach (var s in group.selectedSynergies)
    //         {
    //             if (s != Synergy.Ready)
    //             {
    //                 allReady = false;
    //                 break;
    //             }
    //         }

    //         if (allReady)
    //         {
    //             group.IsCompleted = true;
    //             Debug.Log(group.SynergyName);
    //             attakTowerSetting.attackDamage *= (1 + group.ChangeDamage);
    //             attakTowerSetting.attackDelay *= (1 - group.AttacingSpeed);
    //             BulletSpeed += group.BulletSpeed;
    //             isSlow |= group.Slow;
    //         }
    //     }
    // }
    
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

    private bool IsInSight(GameObject target)
    {
        if (target == gameObject) return false;

        Vector2 origin = transform.position;
        Vector2 targetPos = target.transform.position;
        float dist = Vector2.Distance(origin, targetPos);
        if (dist > 2)
            return false;

        Vector2 dir = (targetPos - origin).normalized;
        RaycastHit2D[] hit = Physics2D.RaycastAll(origin, dir, dist, towerLayer);
        Debug.DrawRay(origin, dir * dist, Color.yellow, 0.2f);

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.gameObject == target && hit[i].distance <= dist)
            {
                return true;
            }
        }
        return false;
    }

    private List<SynergyGroup> DeepCopySynergyGroups(List<SynergyGroup> original)
    {
        List<SynergyGroup> copy = new List<SynergyGroup>();
        foreach (var group in original)
        {
            SynergyGroup newGroup = new SynergyGroup
            {
                SynergyName = group.SynergyName,
                selectedSynergies = new List<Synergy>(group.selectedSynergies),
                ChangeHealth = group.ChangeHealth,
                ChangeDamage = group.ChangeDamage,
                BulletSpeed = group.BulletSpeed,
                AttacingSpeed = group.AttacingSpeed,
                Slow = group.Slow,
                IsCompleted = group.IsCompleted
            };
            copy.Add(newGroup);
        }
        return copy;
    }
}