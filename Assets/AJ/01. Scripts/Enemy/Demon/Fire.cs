using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public ParticleSystem particle;
    private float damage;
    public float FireRange = 1f;
    public LayerMask towerLayerMask;
    public float damageInterval = 0.5f;

    private void Start()
    {
        damage = GetComponent<Arrow>().enemyTypeSetting.enemySO.Damage;
    }
    private void OnEnable()
    {
        if (particle != null)
        {
            particle.Stop();
            particle.Play();
        }
    }
    private void OnDisable()
    {
        particle.Pause();
    }
    private void OnDestroy()
    {
        particle.Stop();
    }

    public void FireAttack()
    {
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, FireRange, towerLayerMask);

        foreach (Collider2D towerCollider in towersInRange)
        {
            GameObject tower = towerCollider.gameObject;

            float distance = Vector3.Distance(transform.position, tower.transform.position);
            StartCoroutine(ContinuousDamage(tower));
        }
    }

    private IEnumerator ContinuousDamage(GameObject tower)
    {
        float elapsedTime = 0f;

        // 총 지속시간 동안 반복
        while (elapsedTime < GetComponent<Arrow>().fireRemainingTime)
        {
            if (tower != null && tower.activeInHierarchy)
            {
                Debug.Log($"타워 '{tower.name}'에게 데미지 {damage} 적용!");

                // 실제 데미지 적용
                // TowerHealth towerHealth = tower.GetComponent<TowerHealth>();
                // if (towerHealth != null)
                // {
                //     towerHealth.TakeDamage(damage);
                // }
            }
            else
            {
                // 타워가 파괴되었으면 지속 데미지 중단
                Debug.Log("타워가 파괴되어 지속 데미지 중단");
                break;
            }

            // 다음 데미지까지 대기
            yield return new WaitForSeconds(damageInterval);
            elapsedTime += damageInterval;
        }

        Debug.Log($"지속 데미지 종료 (총 {elapsedTime:F1}초)");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, FireRange);

        // 범위 내 타워들 표시
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, FireRange, towerLayerMask);
        Gizmos.color = Color.yellow;

        foreach (Collider2D tower in towersInRange)
        {
            Gizmos.DrawLine(transform.position, tower.transform.position);
            Gizmos.DrawWireCube(tower.transform.position, Vector3.one * 0.5f);
        }
    }
}
