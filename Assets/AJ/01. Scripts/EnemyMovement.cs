using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public enum ChooseAttakingType
{
    None = -1,

    Core = 0,
    Tower,

    Max,
}
public class EnemyMovement : MonoBehaviour
{
    public EnemyTypeSetting SO;
    public ChooseAttakingType attackingType;

    [Header("Obstacle Avoidance")]
    public float detectionDistance = 2f;
    public float avoidanceForce = 5f;
    public LayerMask obstacleLayerMask = -1;
    
    [Header("Target Detection")]
    public LayerMask coreLayerMask = -1;
    public LayerMask towerLayerMask = -1;
    public float targetSearchRadius = 10f;
    
    [Header("Search Settings")]
    public bool useUnlimitedRange = true; // 무제한 범위 검색 여부
    public float maxSearchDistance = 100f; // 최대 검색 거리 (무제한이 아닐 때)

    public GameObject target { get; set; }
    private bool isSkeletonStopped = false;
    private EnemyShoot enemyShoot;
    private Vector3 avoidanceDirection = Vector3.zero; 
    private Vector3 smoothedDirection = Vector3.zero;
    private float directionSmoothTime = 0.2f;
    private Vector3 originalScale;

    private void Start()
    {
        FindTarget();
        enemyShoot = GetComponent<EnemyShoot>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        if (SO.enemySO.enemyType == EnemyType.Skelenton || SO.enemySO.enemyType == EnemyType.Demon)
        {
            Collider2D towerCollider = Physics2D.OverlapCircle(transform.position, enemyShoot.detectionRange, enemyShoot.towerLayerMask);
            if (towerCollider != null)
            {
                isSkeletonStopped = true;
                FindTarget();
                LookAtTarget(target.transform.position);
                return;
            }
            else
            {
                isSkeletonStopped = false;
            }
        }

        if (!isSkeletonStopped)
        {
            FindTarget();
            ObstacleFrontEnemy();
            MoveToTarget();
        }
    }
    public void FindTarget()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        LayerMask targetLayerMask = GetTargetLayerMask();

        if (targetLayerMask == 0)
        {
            Debug.LogWarning($"Invalid attacking type: {attackingType}");
            return;
        }
        
        if (useUnlimitedRange)
        {
            // 전체 씬에서 검색
            closestTarget = FindTargetInEntireScene(targetLayerMask);
        }
        else
        {
            // 제한된 범위에서 검색
            closestTarget = FindTargetInRange(targetLayerMask, maxSearchDistance);
        }
        target = closestTarget;

        if (target == null)
        {
            Debug.LogWarning($"No target found for {attackingType}!");
        }
    }
    private GameObject FindTargetInEntireScene(LayerMask targetLayerMask)
    {
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in allGameObjects)
        {
            // 레이어 체크
            if (((1 << obj.layer) & targetLayerMask) != 0)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = obj;
                }
            }
        }

        return closestTarget;
    }
    private GameObject FindTargetInRange(LayerMask targetLayerMask, float searchRange)
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, searchRange, targetLayerMask);

        if (potentialTargets.Length == 0) return null;

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D targetCollider in potentialTargets)
        {
            float distance = Vector3.Distance(transform.position, targetCollider.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = targetCollider.gameObject;
            }
        }

        return closestTarget;
    }
    private LayerMask GetTargetLayerMask()
    {
        switch (attackingType)
        {
            case ChooseAttakingType.Core:
                return coreLayerMask;
            case ChooseAttakingType.Tower:
                return towerLayerMask;
            default:
                return 0;
        }
    }
    /// <summary>
    /// 특정 범위 내 타워를 찾는 함수
    /// </summary>
    /// <param name="searchRange"></param>
    /// <returns></returns>
    public GameObject FindClosestTowerInRange(float searchRange)
    {
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, searchRange, towerLayerMask);

        if (towers.Length == 0) return null;

        GameObject closestTower = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D towerCollider in towers)
        {
            float distance = Vector3.Distance(transform.position, towerCollider.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTower = towerCollider.gameObject;
            }
        }

        return closestTower;
    }
    private void MoveToTarget()
    {
        if (target == null) return;

        Vector3 targetDirection = (target.transform.position - transform.position).normalized;

        Vector3 finalDirection;
        if (avoidanceDirection.magnitude > 0.1f)
        {
            finalDirection = (targetDirection + avoidanceDirection * 0.8f).normalized;
        }
        else
        {
            finalDirection = targetDirection;
        }

        smoothedDirection = Vector3.Lerp(smoothedDirection, finalDirection, directionSmoothTime);

        if (Vector3.Angle(transform.up, smoothedDirection) > 10f)
        {
            LookAtTarget(transform.position + smoothedDirection);
        }

        transform.position += smoothedDirection * SO.enemySO.enemySpeed * Time.deltaTime;
    }

    
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
    private void ObstacleFrontEnemy()
    {
        Vector3 forward = (target.transform.position - transform.position).normalized;

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

        if (angle < 0) angle += 360;

        if ((angle >= 315 || angle <= 45) || (angle >= 135 && angle <= 225))
        {
            if (angle > 90 && angle < 270)
            {
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                transform.rotation = Quaternion.Euler(0, 0, angle + 180);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            if (angle > 45 && angle < 135) 
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                transform.rotation = Quaternion.Euler(0, 0, 0); 
            }
            else if (angle > 225 && angle < 315)
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        //무조건 왼쪽 오른쪽만 보게
        /*if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }*/
    }

    private void OnDrawGizmosSelected()
    {
        // 타겟 검색 범위 표시
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, targetSearchRadius);

        if (target != null)
        {
            Gizmos.color = Color.blue;
            Vector3 forward = (target.transform.position - transform.position).normalized;
            Gizmos.DrawRay(transform.position, forward * detectionDistance);
        }

        if (avoidanceDirection != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, avoidanceDirection);
        }
    }
}