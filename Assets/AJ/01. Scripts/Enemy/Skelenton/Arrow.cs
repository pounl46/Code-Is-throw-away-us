using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    public GameObject core;
    public float bulletSpeed = 5f;
    public float rotateSpeed = 2f;
    
    public EnemyTypeSetting enemyTypeSetting;
    
    private void Update()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        core = GameObject.Find("Core");
        Vector2 target = (core.transform.position - transform.position);
        float rotZ = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        Vector3 direction = target.normalized;
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            gameObject.SetActive(false);
        }
    }

}
