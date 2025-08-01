using System;
using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int Health { get; private set; }
    public EnemyTypeSetting enemyTypeSetting;

    private bool isDead;
    public event Action OnDamage;
    public event Action OnDead;

    public GameObject slimePrefab;
    private void Awake()
    {
        Health = enemyTypeSetting.enemySO.enemyHealth;
    }

    public void Damage(int damage, GameObject dealer)
    {
        Health -= damage;
        Health = Mathf.Clamp(Health, 0, enemyTypeSetting.enemySO.enemyHealth);
        OnDamage?.Invoke();

        if (Health <= 0)
        {
            Health = 0;
            isDead = true;
            OnDead?.Invoke();
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return null;
        if (enemyTypeSetting.enemySO.enemyType == EnemyType.BigSlime || enemyTypeSetting.enemySO.enemyType == EnemyType.KingSlime)
        {
            CloneSlime();
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void CloneSlime()
    {
        int randomSlimeSpawn = UnityEngine.Random.Range(1, 6);
        for (int i = 0; i < randomSlimeSpawn; i++)
        {
            GameObject slime = Instantiate(slimePrefab);
            slime.transform.position = transform.position;
        }
    }
}
