using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public EnemyTypeSetting SO;

    private EnemyType Type;
    private HealthSystem _healthSystem;
    public float HpRatio
    {
        get => (float)_healthSystem.Health / SO.enemySO.enemyHealth;
    }
    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
    }
    //1. 플레이어 체력만큼 UI만들기
    private void Start()

    {
        _healthSystem.OnDamage += UpdateHealthUI;
    }

    private void UpdateHealthUI()
    {
        //UIManager.Instance.HealthUI.UpdateHealthUI(_healthSystem.Health);
    }
    private void IsDead()
    {

    }
    private void OnDestroy()
    {
        _healthSystem.OnDamage -= UpdateHealthUI; 
    }
    private void OnValidate()
    {
        Type = SO.enemySO.enemyType;
    }
}
