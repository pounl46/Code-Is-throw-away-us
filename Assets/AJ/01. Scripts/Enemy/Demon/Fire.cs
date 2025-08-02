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

        // �� ���ӽð� ���� �ݺ�
        while (elapsedTime < GetComponent<Arrow>().fireRemainingTime)
        {
            if (tower != null && tower.activeInHierarchy)
            {
                Debug.Log($"Ÿ�� '{tower.name}'���� ������ {damage} ����!");

                // ���� ������ ����
                // TowerHealth towerHealth = tower.GetComponent<TowerHealth>();
                // if (towerHealth != null)
                // {
                //     towerHealth.TakeDamage(damage);
                // }
            }
            else
            {
                // Ÿ���� �ı��Ǿ����� ���� ������ �ߴ�
                Debug.Log("Ÿ���� �ı��Ǿ� ���� ������ �ߴ�");
                break;
            }

            // ���� ���������� ���
            yield return new WaitForSeconds(damageInterval);
            elapsedTime += damageInterval;
        }

        Debug.Log($"���� ������ ���� (�� {elapsedTime:F1}��)");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, FireRange);

        // ���� �� Ÿ���� ǥ��
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, FireRange, towerLayerMask);
        Gizmos.color = Color.yellow;

        foreach (Collider2D tower in towersInRange)
        {
            Gizmos.DrawLine(transform.position, tower.transform.position);
            Gizmos.DrawWireCube(tower.transform.position, Vector3.one * 0.5f);
        }
    }
}
