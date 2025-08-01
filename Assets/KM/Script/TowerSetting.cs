using UnityEditor;
using Script.SO;
using UnityEngine;

public class TowerSetting : MonoBehaviour
{
    [Header("Tower Health")]
    public float Health;

    [Header("Get Tower SO")]
    public AttakTowerSetting _attakTowerSetting;

    [Header("Get Tower SpriteRenderer")]
    [SerializeField] private SpriteRenderer _renderer;

    [Header("Get AudioSource In Tower")]
    [SerializeField] private AudioSource fire;

    public float attackDamage;
    public Synergy TowerType;
    public float attackDelay;
    public float attackDistance;
    private void OnValidate()
    {
        fire.clip = _attakTowerSetting.Audio;
        gameObject.name = _attakTowerSetting.name;
        Health = _attakTowerSetting.TowerHealth;
        TowerType = _attakTowerSetting.synergy;
        if (_renderer != null)
        {
            _renderer.sprite = _attakTowerSetting.TowerSprite;
        }
        attackDamage = _attakTowerSetting.AttackDamage;
        attackDelay = _attakTowerSetting.AttackDelay;
        attackDistance = _attakTowerSetting.AttackDistance;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
#endif
}
