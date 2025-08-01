using UnityEngine;
using UnityEngine.U2D.Animation;

public class Walk : MonoBehaviour
{
    private Animator animator;

    [SerializeField] SpriteLibrary spriteLibary;
    public EnemyTypeSetting SO;

    private bool isDead = false;
    private readonly int _DeadHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead) return;
        
    }
    private void OnValidate()
    {
        if (SO.enemySO == null) return;
        gameObject.name = SO.enemySO.enemyName;
        if (spriteLibary != null)
            spriteLibary.spriteLibraryAsset = SO.enemySO.spriteAsset;
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
