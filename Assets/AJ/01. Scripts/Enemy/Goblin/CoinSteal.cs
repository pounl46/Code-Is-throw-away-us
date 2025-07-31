using System.Collections;
using UnityEngine;

public class CoinSteal : MonoBehaviour
{
    public ChooseAttakingType attackingType;
    [Header("Steal Settings")]
    public float stealDuration = 10f;
    public float fleeSpeed = 4f;

    private bool isStealing = false;
    private bool isFleeing = false;
    private GameObject targetTower;
    private Vector3 fleeDirection;
    private EnemyMovement enemyMovement;
    private Vector3 originalScale; 
    private Transform spawnPointTarget; 

    
    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        if (enemyMovement == null)
        {
            Debug.LogError("EnemyMovement component not found!");
        }
        originalScale = transform.localScale;

        targetTower = FindClosestTower();
        if (targetTower != null)
        {
            enemyMovement.SetTarget(targetTower);
        }
    }

    private void Update()
    {
        if (isFleeing)
        {
            FleeFromTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStealing || isFleeing)
        {
            return;
        }

        if (collision.CompareTag(attackingType.ToString()))
        {
            if (collision.gameObject == targetTower)
            {
                StartStealing();
            }
            else
            {
                Debug.Log($"다른 타워와 충돌: {collision.gameObject.name}");
            }
        }

    }
    private GameObject FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject tower in towers)
        {
            float distance = Vector3.Distance(currentPos, tower.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = tower;
            }
        }

        return closest;
    }
    private void StartStealing()
    {
        if (targetTower == null) return;

        isStealing = true;

        if (enemyMovement != null)
        {
            enemyMovement.enabled = false;
        }

        // 도망갈 방향 설정
        fleeDirection = (transform.position - targetTower.transform.position).normalized;

        // spawnPointTarget이 설정되어 있는지 확인
        if (spawnPointTarget == null)
        {
            return;
        }

        StartCoroutine(StealCoroutine());

    }

    private IEnumerator StealCoroutine()
    {
        float timeElapsed = 0f;

        while (timeElapsed < stealDuration)
        {
            StealCoin();
            yield return new WaitForSeconds(1f);
            timeElapsed += 1f;
        }

        StartFleeing();
    }

    private void StealCoin()
    {
        if (targetTower == null) return;

        // CoinTower 코인 훔치기
        Debug.Log($"{gameObject.name}가 코인을 훔침!");
    }

    private void StartFleeing()
    {
        isStealing = false;
        isFleeing = true;
    }

    private void FleeFromTarget()
    {
        if (spawnPointTarget == null) return;

        // 스폰 포인트 방향으로 도망가기
        Vector3 directionToSpawn = (spawnPointTarget.position - transform.position).normalized;
        transform.position += directionToSpawn * fleeSpeed * Time.deltaTime;

        LookAtDirection(directionToSpawn);

        float distance = Vector3.Distance(transform.position, spawnPointTarget.position);
        if (distance < 1f)
        {
            ReturnToNormalMovement();
        }

    }
    public void SetSpawnPoint(Transform spawnPoint)
    {
        spawnPointTarget = spawnPoint;
    }


    private void ReturnToNormalMovement()
    {
        isFleeing = false;
        isStealing = false;

        if (enemyMovement != null)
        {
            enemyMovement.enabled = true;

            targetTower = FindClosestTower();
            if (targetTower != null)
            {
                enemyMovement.SetTarget(targetTower);
            }
        }

    }

    private void LookAtDirection(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (targetTower != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, targetTower.transform.position);
        }

        if (isFleeing && fleeDirection != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, fleeDirection * 3f);
        }

        if (isStealing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
        else if (isFleeing)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}