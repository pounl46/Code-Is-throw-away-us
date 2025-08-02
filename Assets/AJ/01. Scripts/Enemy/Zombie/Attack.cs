using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public ChooseAttakingType attackingType;
    private bool isAttacking = false;
    private EnemyMovement enemyMv;
    private bool isAttackCoroutineRunning = false;

    public float attackRange = 1f;
    public float rushToTarget = 2f;
    public float backingTime = 1f;

    public float damage;

    private GameObject tower; // 충돌한 타워 저장

    public float findTower;

    public EnemyTypeSetting enemyType;
    private void Start()
    {
        enemyMv = GetComponent<EnemyMovement>();
        enemyType = GetComponent<EnemyTypeSetting>();
        damage = enemyType.enemySO.Damage;
        if (enemyMv == null)
        {
            Debug.LogError("EnemyMovement component not found!");
        }
    }
    private void Update()
    {
        if (isAttacking && !isAttackCoroutineRunning)
        {
            StartCoroutine(BladeAttack());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(attackingType.ToString()))
        {
            isAttacking = true;
            
            tower = collision.gameObject;

            enemyMv.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(attackingType.ToString()))
        {
            isAttacking = false;
            tower = null;
            StartCoroutine(WaitForAttackEnd());
        }
    }

    private IEnumerator BladeAttack()
    {
        isAttackCoroutineRunning = true;

        while (isAttacking)
        {
            enemyMv.enabled = false;

            GameObject nearTower = FindNearestTower();
            TowerHealthManager towerHealth = tower.GetComponent<TowerHealthManager>();
            if (towerHealth != null)
            {
                towerHealth.OnDamamge(Mathf.RoundToInt(damage));
                Debug.Log(towerHealth.nowTowerHealth);
            }

            Vector3 movementDirection = enemyMv.GetMovementDirection();
            if (movementDirection == Vector3.zero && enemyMv.target != null)
            {
                movementDirection = (enemyMv.target.transform.position - transform.position).normalized;
            }

            Vector3 backDirection = -movementDirection;
            Vector3 startPosition = transform.position;
            Vector3 targetBackPosition = startPosition + backDirection * attackRange;

            float elapsedTime = 0f;
            while (elapsedTime < backingTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / backingTime;

                t = 1f - (1f - t) * (1f - t);

                transform.position = Vector3.Lerp(startPosition, targetBackPosition, t);
                yield return null;
            }

            transform.position = targetBackPosition;

            enemyMv.speedMagnification = rushToTarget;
            enemyMv.enabled = true;

            yield return new WaitForSeconds(0.1f);
        }

        isAttackCoroutineRunning = false;
    }
    private GameObject FindNearestTower()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, findTower);

        GameObject nearestTower = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            // TowerHealthManager 컴포넌트가 있는지 확인
            TowerHealthManager towerHealth = col.GetComponent<TowerHealthManager>();
            if (towerHealth != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTower = col.gameObject;
                }
            }
        }

        if (nearestTower != null)
        {
            Debug.Log($"Found nearest tower: {nearestTower.name} at distance: {nearestDistance}");
        }

        return nearestTower;
    }
    private IEnumerator WaitForAttackEnd()
    {
        while (isAttackCoroutineRunning)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(backingTime);

        enemyMv.speedMagnification = 1f;
        enemyMv.enabled = true;
    }
    private void OnDrawGizmosSelected()
    {
        if (isAttacking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 3);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, findTower);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 3);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, findTower);
        }
    }
}
