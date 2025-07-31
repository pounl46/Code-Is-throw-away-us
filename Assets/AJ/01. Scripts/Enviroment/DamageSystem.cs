using System;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public Action<int> OnDamageTaken;
    public Action OnDeath;
    
    private int currentHealth;
    private EnemyTypeSetting enemyTypeSetting;
    
    private void Awake()
    {
        enemyTypeSetting = GetComponent<EnemyTypeSetting>();
    }
    
    private void OnEnable()
    {
        currentHealth = enemyTypeSetting.enemySO.enemyHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        OnDamageTaken?.Invoke(damage);
        
        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} 파괴됨!");
            OnDeath?.Invoke();
        }
    }
    
}
