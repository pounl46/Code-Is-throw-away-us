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

    private void Start()
    {
        enemyMv = GetComponent<EnemyMovement>();
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
            enemyMv.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(attackingType.ToString()))
        {
            isAttacking = false;
            StartCoroutine(WaitForAttackEnd());
        }
    }

    private IEnumerator BladeAttack()
    {
        isAttackCoroutineRunning = true;

        while (isAttacking)
        {
            enemyMv.enabled = false;
            Vector3 currentPosition = transform.position;

            Vector3 movementDirection = enemyMv.GetMovementDirection();

            if (movementDirection == Vector3.zero && enemyMv.target != null)
            {
                movementDirection = (enemyMv.target.transform.position - transform.position).normalized;
            }

            Vector3 backDirection = -movementDirection * attackRange;
            Vector3 backPosition = currentPosition + backDirection;

            transform.DOMove(backPosition, backingTime).SetEase(Ease.Linear);

            yield return new WaitForSeconds(backingTime);

            enemyMv.speedMagnification = rushToTarget;
            enemyMv.enabled = true;
            Debug.Log($"{gameObject.name}µ•πÃ¡ˆ ¥‚¿Ω");
            yield return new WaitForSeconds(0.1f);
        }

        isAttackCoroutineRunning = false;
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
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 3);
        }
    }
}
