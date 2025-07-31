using UnityEngine;
using UnityEngine.InputSystem;

public class GiveDamage : MonoBehaviour
{
    public int Damage;
    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));
        transform.position = mousePos;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable Idamageable))
        {
            Idamageable.Damage(Damage, gameObject);
            Debug.Log($"«« {Damage}±Ô¿”");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable Idamageable))
        {
            Idamageable.Damage(Damage, gameObject);
            Debug.Log($"«« {Damage}±Ô¿”");
        }
    }
}
