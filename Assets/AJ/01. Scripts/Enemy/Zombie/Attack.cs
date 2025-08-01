using System;
using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public ChooseAttakingType attackingType;
    public GameObject blade;
    public float attackInterval;
    private bool isAttacking = false;
    private EnemyMovement enemyMv;
    private bool isAttackCoroutineRunning = false;

    public float attackRange = 1f;

    private void Start()
    {
        enemyMv = GetComponent<EnemyMovement>();
        if (enemyMv == null)
        {
            Debug.LogError("EnemyMovement component not found!");
        }
        if (enemyMv.SO.enemySO.enemyType == EnemyType.Zombie || enemyMv.SO.enemySO.enemyType == EnemyType.Bat || enemyMv.SO.enemySO.enemyType == EnemyType.Ghost)
            blade.SetActive(false);
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
            enemyMv.enabled = true;
            blade.SetActive(false);
        }
    }

    private IEnumerator BladeAttack()
    {
        isAttackCoroutineRunning = true;

        while (isAttacking)
        {
            blade.SetActive(true);
            //Here is tower health jhonaa a pha

            yield return new WaitForSeconds(attackInterval);

            blade.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        isAttackCoroutineRunning = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (isAttacking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
