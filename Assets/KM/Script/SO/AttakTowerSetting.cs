using UnityEngine;

[CreateAssetMenu(fileName = "AttakTowerSetting", menuName = "SO/AttakTowerSetting")]
public class AttakTowerSetting : ScriptableObject
{
    public string Name;
    public float AttackDamage;
    public float AttackDelay;
    public float AttackDistance;
    public Sprite TowerSprite;
    public float TowerHealth;
    public AudioClip Audio;

    public int Cost;
}
