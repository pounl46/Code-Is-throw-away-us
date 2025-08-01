using System.Collections;
using UnityEngine;

public class ThrowScript : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    public GameObject hitEffect;
    private TowerAttack attack;
    private bool IsAttacking;
    private void Awake()
    {
        attack = transform.parent.transform.GetComponent<TowerAttack>();
        IsAttacking = true;
        StartCoroutine(DeadSelf());
    }
    private void OnEnable()
    {
        IsAttacking = true;
        StartCoroutine(DeadSelf());
    }

    private void FixedUpdate()
    {
        if (IsAttacking)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.25f, _targetMask);
            if (hit)
            {
                //적 맞췄을떄 이벤트

                IsAttacking = false;
                Instantiate(hitEffect, transform.parent).transform.position = transform.position;
                attack.throws.Push(gameObject);
                StopAllCoroutines();
                gameObject.SetActive(false);
            }
        }
    }
    private IEnumerator DeadSelf()
    {
        yield return new WaitForSeconds(3);
        attack.throws.Push(gameObject);
        IsAttacking = false;
        gameObject.SetActive(false);
    }
}
