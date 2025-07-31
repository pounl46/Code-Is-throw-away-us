using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyTypeSetting SO;

    private EnemyType Type;
    private void OnValidate()
    {
        Type = SO.enemySO.enemyType;
        EnemyTypes();
    }

    
    private void EnemyTypes()
    {
        if (Type == EnemyType.None)
        {
            return;
        }
        else if (Type == EnemyType.Skelenton)
        {
            gameObject.GetComponent<EnemyShoot>().enabled = true;
        }
        else if (Type == EnemyType.Goblin)
        {
            gameObject.GetComponent<CoinSteal>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<EnemyShoot>().enabled = false;
        }
    }
}
