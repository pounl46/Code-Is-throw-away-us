using NUnit.Framework;
using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyTypeSetting SO;
    public ChooseAttakingType attackingType;

    [Header("Obstacle Avoidance")]
    public float detectionDistance = 2f;
    public float avoidanceForce = 5f;
    public LayerMask obstacleLayerMask = -1;
    private GameObject core;

    private bool isSkeletonStopped = false;
    private EnemyShoot enemyShoot;
    private Vector3 avoidanceDirection = Vector3.zero; private Vector3 smoothedDirection = Vector3.zero;
    private float directionSmoothTime = 0.2f;

    private void Start()
    {
        FindCore();
        enemyShoot = GetComponent<EnemyShoot>();
    }

    private void Update()
    {
        if (core == null)
        {
            FindCore();
            return;
        }

        if (SO.enemySO.enemyType == EnemyType.Skelenton)
        {
            Collider2D towerCollider = Physics2D.OverlapCircle(transform.position, enemyShoot.detectionRange, enemyShoot.towerLayerMask);
            if (towerCollider != null)
            {
                isSkeletonStopped = true;
                LookAtTarget(core.transform.position);
                return;
            }
            else
            {
                isSkeletonStopped = false;
            }
        }

        if (!isSkeletonStopped)
        {
            ObstacleFrontEnemy();
            MoveToCore();
        }
    }

    private void MoveToCore()
    {
        Vector3 coreDirection = (core.transform.position - transform.position).normalized;

        // 회피 방향이 있으면 합치고, 없으면 코어 방향만 사용
        Vector3 targetDirection;
        if (avoidanceDirection.magnitude > 0.1f)
        {
            targetDirection = (coreDirection + avoidanceDirection * 0.8f).normalized;
        }
        else
        {
            targetDirection = coreDirection;
        }

        // 방향을 부드럽게 변경 (지터링 방지)
        smoothedDirection = Vector3.Lerp(smoothedDirection, targetDirection, directionSmoothTime);

        // 실제 이동할 방향을 바라보기 (너무 자주 회전하지 않도록)
        if (Vector3.Angle(transform.up, smoothedDirection) > 10f)
        {
            LookAtTarget(transform.position + smoothedDirection);
        }

        transform.position += smoothedDirection * SO.enemySO.enemySpeed * Time.deltaTime;
    }

    private void FindCore()
    {
        core = GameObject.FindGameObjectWithTag(attackingType.ToString());
        if (core == null)
        {
            Debug.LogWarning("core is null!");
        }
    }

    private void ObstacleFrontEnemy()
    {
        Vector3 forward = (core.transform.position - transform.position).normalized;

        // 고정된 방향으로 레이캐스트 (월드 좌표 기준)
        Vector3 rightDirection = Vector3.right;
        Vector3 leftDirection = Vector3.left;
        Vector3 upDirection = Vector3.up;
        Vector3 downDirection = Vector3.down;

        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, forward, detectionDistance, obstacleLayerMask);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, rightDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, leftDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D upHit = Physics2D.Raycast(transform.position, upDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D downHit = Physics2D.Raycast(transform.position, downDirection, detectionDistance * 0.8f, obstacleLayerMask);

        Debug.DrawRay(transform.position, forward * detectionDistance, Color.red);
        Debug.DrawRay(transform.position, rightDirection * detectionDistance * 0.8f, Color.yellow);
        Debug.DrawRay(transform.position, leftDirection * detectionDistance * 0.8f, Color.yellow);
        Debug.DrawRay(transform.position, upDirection * detectionDistance * 0.8f, Color.blue);
        Debug.DrawRay(transform.position, downDirection * detectionDistance * 0.8f, Color.blue);

        Vector3 newAvoidDirection = Vector3.zero;

        // 앞에 장애물이 있을 때만 회피 계산
        if (forwardHit.collider != null)
        {
            // 4방향 중 가장 비어있는 방향 찾기
            bool rightClear = rightHit.collider == null;
            bool leftClear = leftHit.collider == null;
            bool upClear = upHit.collider == null;
            bool downClear = downHit.collider == null;

            // 우선순위: 좌우 > 상하
            if (rightClear && !leftClear)
            {
                newAvoidDirection = Vector3.right;
            }
            else if (leftClear && !rightClear)
            {
                newAvoidDirection = Vector3.left;
            }
            else if (rightClear && leftClear)
            {
                // 둘 다 비어있으면 코어와 수직인 방향으로
                Vector3 perpendicular = Vector3.Cross(forward, Vector3.forward).normalized;
                newAvoidDirection = perpendicular;
            }
            else if (upClear)
            {
                newAvoidDirection = Vector3.up;
            }
            else if (downClear)
            {
                newAvoidDirection = Vector3.down;
            }
            else
            {
                // 모든 방향이 막혔으면 뒤로
                newAvoidDirection = -forward;
            }
        }

        if (newAvoidDirection.magnitude > 0.1f)
        {
            Vector3 targetAvoidance = newAvoidDirection * avoidanceForce;
            avoidanceDirection = Vector3.Lerp(avoidanceDirection, targetAvoidance, Time.deltaTime * 4f);
        }
        else
        {
            avoidanceDirection = Vector3.Lerp(avoidanceDirection, Vector3.zero, Time.deltaTime * 5f);
        }
    }

    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector2 direction = targetPosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 forward = (core != null) ? (core.transform.position - transform.position).normalized : transform.right;
        Gizmos.DrawRay(transform.position, forward * detectionDistance);

        if (avoidanceDirection != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, avoidanceDirection);
        }
    }
}
public enum ChooseAttakingType
{
    None = -1,

    Core = 0,
    Tower,

    Max,
}