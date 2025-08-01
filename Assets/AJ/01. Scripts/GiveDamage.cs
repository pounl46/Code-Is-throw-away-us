using UnityEngine;
using UnityEngine.InputSystem;

public class GiveDamage : MonoBehaviour
{
    public static GiveDamage Instance { get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }       
    }
    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));
        transform.position = mousePos;
    }
    public void CallDamage(int Damage, GameObject dealer)
    {
        dealer.GetComponent<IDamageable>().Damage(Damage,dealer);
    }
}
