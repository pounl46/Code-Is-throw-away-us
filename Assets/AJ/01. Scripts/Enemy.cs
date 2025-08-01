using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    private HealthSystem healthSystem;
    private EnemyTypeSetting enemyTypeSetting;

    private EnemyType Type;
    public float HpRatio=> (float)healthSystem.Health / enemyTypeSetting.enemySO.enemyHealth;
    public bool IsAlive => healthSystem.Health > 0;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (enemyTypeSetting == null)
            enemyTypeSetting = GetComponent<EnemyTypeSetting>();
    }
    private void Start()
    {
        if (EnemyHpBarManager.Instance != null)
        {
            EnemyHpBarManager.Instance.AddHpBarToEnemy(this);
        }
        healthSystem.OnDamage += Damaged;
    }

    private void Damaged()
    {
        StartCoroutine(EnemyColorChange());
    }

    private IEnumerator EnemyColorChange()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnDestroy()
    {
        healthSystem.OnDamage -= Damaged;
        if (EnemyHpBarManager.Instance != null)
        {
            EnemyHpBarManager.Instance.RemoveHpBarFromEnemy(this);
        }
    }
    private void OnValidate()
    {
        if (enemyTypeSetting == null)
            enemyTypeSetting = GetComponent<EnemyTypeSetting>();

        if (enemyTypeSetting != null && enemyTypeSetting.enemySO != null)
        {
            Type = enemyTypeSetting.enemySO.enemyType;
        }
    }
}
