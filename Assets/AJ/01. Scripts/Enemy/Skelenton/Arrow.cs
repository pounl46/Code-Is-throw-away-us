using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float bulletSpeed = 5f;
    public float rotateSpeed = 2f;
    public float fireRemainingTime = 1f;
    public float bulletDamage;

    public EnemyMovement enemymv;

    public EnemyTypeSetting enemyTypeSetting;
    public EnemyShoot enemyShoot;

    private GameObject target;
    public float originalBulletSpeed { get; private set; }

    private Animator animator;
    private readonly int isTowerHash = Animator.StringToHash("IsTower");

    private Bomb bomb;

    public ParticleSystem particle;
    private void Awake()
    {
        originalBulletSpeed = bulletSpeed;
        bulletDamage = enemymv.SO.enemySO.Damage;
        animator = GetComponent<Animator>();
        bomb = GetComponent<Bomb>();
    }
    private void OnEnable()
    {
        if (enemyShoot != null)
        {
            target = enemyShoot.GetCurrentTarget();
        }
        if (animator != null)
        {
            animator.SetBool(isTowerHash, false);
        }
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OnDisable()
    {
        bulletSpeed = originalBulletSpeed;
        target = null;
        if (animator != null)
        {
            animator.SetBool("IsTower", false);
        }
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
            if (enemyTypeSetting.enemySO.enemyType == EnemyType.Demon || enemyTypeSetting.enemySO.enemyType == EnemyType.BombGoblin)
            {
                if (enemyTypeSetting.enemySO.enemyType == EnemyType.BombGoblin)
                {
                    StartCoroutine(BombTiming());
                }
                else if(enemyTypeSetting.enemySO.enemyType == EnemyType.Demon)
                {
                    StartCoroutine(FireRemaining());
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    private IEnumerator BombTiming()
    {
        bulletSpeed = 0f;

        if (animator != null)
        {
            animator.SetBool(isTowerHash, true);
        }

        yield return new WaitForSeconds(fireRemainingTime);

        if (animator != null)
        {
            animator.SetBool(isTowerHash, false);
        }
        
        GetComponent<SpriteRenderer>().enabled = false;

        if (particle != null)
        {
            particle.Stop();
            particle.Play();
            yield return new WaitForSeconds(1f);
        }

        bomb.Explode();

        gameObject.SetActive(false);
    }
    private IEnumerator FireRemaining()
    {
        bulletSpeed = 0f;
        if (GetComponent<Fire>() != null)
            GetComponent<Fire>().FireAttack();
        yield return new WaitForSeconds(fireRemainingTime);
        gameObject.SetActive(false);
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

}
