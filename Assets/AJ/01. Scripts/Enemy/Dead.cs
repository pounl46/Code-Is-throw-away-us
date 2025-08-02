using UnityEngine;

public class Dead : MonoBehaviour
{
    private HealthSystem healthSystem;
    private Animator animator;
    private readonly int _DeadHash = Animator.StringToHash("Dead");
    public bool isDead { get; set; } = false;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        healthSystem.OnDead += OnDead;
    }
    public void OnDead()
    {
        isDead = true;
        gameObject.GetComponent<EnemyMovement>().enabled = false;
        animator.SetBool(_DeadHash, true);
        Debug.Log($"{gameObject.name} ����!");
    }
    public void ButtonClick()
    {
        healthSystem.Damage(100, gameObject);
        Debug.Log($"{gameObject.name}������ 100 ����");
    }
    private void OnDestroy()
    {
        healthSystem.OnDead -= OnDead;
    }
}
