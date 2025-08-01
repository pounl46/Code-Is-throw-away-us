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

    private void Start()
    {
        InitializeArrowPool();
    }

    private void InitializeArrowPool()
    {
        arrowParent = GameObject.FindGameObjectWithTag("BulletTransform").transform;
        _arrowPool = new GameObject[_bulletCount];
        for (int i = 0; i < _bulletCount; i++)
        {
            _arrowPool[i] = Instantiate(arrowPrefab, arrowParent);
            _arrowPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        if(_canFire && IsTowerNearby()) Fire();
    }
    private void Fire()
    {
        for (int i = 0; i < _bulletCount; i++)
        {
            if (!_arrowPool[i].activeSelf)
            {
                _arrowPool[i].SetActive(true);
                _arrowPool[i].transform.position = firePos.position;
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
    private bool IsTowerNearby()
    {
        Collider2D towerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, towerLayerMask);
        
        return towerCollider != null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
