using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float bulletSpeed = 5f;
    public float rotateSpeed = 2f;
    public float fireRemainingTime = 1f;

    public EnemyTypeSetting enemyTypeSetting;
    public EnemyShoot enemyShoot;

    private GameObject target;
    public float originalBulletSpeed { get; private set; }

    private void Awake()
    {
        originalBulletSpeed = bulletSpeed;
    }

    private void OnEnable()
    {
        if (enemyShoot != null)
        {
            target = enemyShoot.GetCurrentTarget();
        }
    }

    private void OnDisable()
    {
        bulletSpeed = originalBulletSpeed;
        target = null;
    }

    private void Update()
    {
        if (target != null && target.activeInHierarchy)
        {
            MoveBullet();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void MoveBullet()
    {
        Vector2 targetDistance = (target.transform.position - transform.position);
        float rotZ = Mathf.Atan2(targetDistance.y, targetDistance.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        Vector3 direction = targetDistance.normalized;
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == target)
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

    private IEnumerator FireRemaining()
    {
        bulletSpeed = 0f;
        yield return new WaitForSeconds(fireRemainingTime);
        gameObject.SetActive(false);
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

}
