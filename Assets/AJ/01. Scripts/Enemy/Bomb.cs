using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionRange = 3f;
    public float damage;
    public LayerMask towerLayerMask = -1;
    public EnemyTypeSetting enemyType;
    private void Start()
    {
        damage = enemyType.enemySO.Damage;
    }

    public void Explode()
    {
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, explosionRange, towerLayerMask);

        foreach (Collider2D towerCollider in towersInRange)
        {
            GameObject tower = towerCollider.gameObject;

            float distance = Vector3.Distance(transform.position, tower.transform.position);

            Debug.Log($"타워 '{tower.name}'에게 데미지 {damage} 적용! (거리: {distance:F2})");

            TowerHealthManager towerHealth = tower.GetComponent<TowerHealthManager>();
            if (towerHealth != null)
            {
                towerHealth.OnDamamge(Mathf.RoundToInt(damage));
                Debug.Log(towerHealth.nowTowerHealth);
            }
        }
    }
    public int GetTowersInRangeCount()
    {
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, explosionRange, towerLayerMask);
        return towersInRange.Length;
    }

    public List<GameObject> GetTowersInRange()
    {
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, explosionRange, towerLayerMask);
        List<GameObject> towers = new List<GameObject>();

        foreach (Collider2D towerCollider in towersInRange)
        {
            towers.Add(towerCollider.gameObject);
        }

        return towers;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);

        // 범위 내 타워들 표시
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, explosionRange, towerLayerMask);
        Gizmos.color = Color.yellow;

        foreach (Collider2D tower in towersInRange)
        {
            Gizmos.DrawLine(transform.position, tower.transform.position);
            Gizmos.DrawWireCube(tower.transform.position, Vector3.one * 0.5f);
        }
    }
}
