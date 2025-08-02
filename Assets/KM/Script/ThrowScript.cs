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
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.25f,Vector2.zero,1,_targetMask);
            if (hit)
            {
                GiveDamage.Instance.CallDamage(Mathf.RoundToInt(transform.parent.GetComponent<TowerSetting>().attackDamage),hit.collider.gameObject);
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
