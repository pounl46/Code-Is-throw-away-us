using UnityEngine;
using UnityEngine.U2D.Animation;
[CreateAssetMenu(fileName = "EnemySO", menuName = "EnemyTypeSetting/EnemySO")]
public class EnemySO : ScriptableObject
{
    public int enemyHealth;
    public float enemySpeed;
    public int Damage;
    public string enemyName;

    public SpriteLibraryAsset spriteAsset;
    public EnemyType enemyType;
}
public enum EnemyType
{
    None = -1,

    Skelenton = 0,
    Zombie,
    Bat,
    Demon,
    Slime,
    Goblin,
    BigSlime,

    Max,
}