using System.Collections;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePos;
    public Transform arrowParent;

    [Header("ArrowCoolTime")]
    public float _coolTime = 0.2f;

    [Header("PoolAmount")]
    public int _bulletCount = 20;

    private bool _canFire = true;
    private GameObject[] _arrowPool;

    [Header("Detection")]
    public float detectionRange = 5f;
    public LayerMask towerLayerMask = -1;
    public LayerMask coreLayerMask = -1;

    private EnemyMovement enemyMovement;

    private GameObject currentTarget;

    private void Start()
    {
        InitializeArrowPool();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        UpdateCurrentTarget();
        if (_canFire && IsTargetNearby())
        {
            Fire();
        }
    }

    private void InitializeArrowPool()
    {
        arrowParent = GameObject.FindGameObjectWithTag("BulletTransform").transform;
        
        _arrowPool = new GameObject[_bulletCount];
        for (int i = 0; i < _bulletCount; i++)
        {
            if (arrowParent != null)
            {
                _arrowPool[i] = Instantiate(arrowPrefab, arrowParent);
            }
            _arrowPool[i].SetActive(false);
        }
    }
    private void UpdateCurrentTarget()
    {
        if (enemyMovement != null)
        {
            GameObject target = enemyMovement.target;
            currentTarget = target;
        }
    }
    private LayerMask GetTargetLayerMask()
    {
        if (enemyMovement == null) return towerLayerMask;

        switch (enemyMovement.attackingType)
        {
            case ChooseAttakingType.Core:
                return coreLayerMask;
            case ChooseAttakingType.Tower:
                return towerLayerMask;
            default:
                return towerLayerMask;
        }
    }
    private void Fire()
    {
        for (int i = 0; i < _bulletCount; i++)
        {
            if (!_arrowPool[i].activeSelf)
            {
                if (gameObject.GetComponent<Dead>().isDead)
                {
                    _arrowPool[i].SetActive(false);
                    break;
                }
                _arrowPool[i].SetActive(true);
                _arrowPool[i].transform.position = firePos.position;

                Arrow arrow = _arrowPool[i].GetComponent<Arrow>();
                if (arrow != null)
                {
                    arrow.enemyShoot = this;
                    arrow.SetTarget(currentTarget);
                }
                break;
            }
        }

        StartCoroutine(CoolTimeCoroutine());
    }

    private IEnumerator CoolTimeCoroutine()
    {
        _canFire = false;
        yield return new WaitForSeconds(_coolTime);
        _canFire = true;
    }
    public bool IsTargetNearby()
    {
        LayerMask targetLayerMask = GetTargetLayerMask();

        if (targetLayerMask == 0) return false;

        Collider2D targetCollider = Physics2D.OverlapCircle(transform.position, detectionRange, targetLayerMask);
        return targetCollider != null;
    }
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
