using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private GameObject core;
    public float bulletSpeed = 5f;
    public float rotateSpeed = 2f;
    public ChooseAttakingType attakingType;
    public float fireRemainingTime = 1f;
    
    public EnemyTypeSetting enemyTypeSetting;

    private bool isFired = false;
    public float originalBulletSpeed { get; private set; } 
    private void Awake()
    {
        originalBulletSpeed = bulletSpeed;
    }
    private void OnEnable()
    {
        core = GameObject.FindGameObjectWithTag(attakingType.ToString());
    }
    private void OnDisable()
    {
        bulletSpeed = originalBulletSpeed;
    }
    private void Update()
    {
        if (core != null)
        {
            MoveBullet();
        }
    }
    private IEnumerator FireRemaining()
    {
        bulletSpeed = 0f;
        yield return new WaitForSeconds(fireRemainingTime);
        gameObject.SetActive(false);
    }
    private void MoveBullet()
    {
        Vector2 target = (core.transform.position - transform.position);
        float rotZ = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        Vector3 direction = target.normalized;
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Core"))
        {
            if (enemyTypeSetting.enemySO.enemyType == EnemyType.Demon)
            {
                StartCoroutine(FireRemaining());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
