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
    public bool useUnlimitedRange = true; // ������ ���� �˻� ����
    public float maxSearchDistance = 100f; // �ִ� �˻� �Ÿ� (�������� �ƴ� ��)
    public bool stuckPrevention;

    public GameObject target { get; set; }
    private bool isSkeletonStopped = false;
    private EnemyShoot enemyShoot;
    private Vector3 avoidanceDirection = Vector3.zero; 
    private Vector3 smoothedDirection = Vector3.zero;
    private float directionSmoothTime = 0.2f;
    private Vector3 originalScale;

    private Collider2D enemyCollider;
    public float speedMagnification = 1f;

    private void Awake()
    {
        enemyCollider = GetComponent<Collider2D>();
    }
    private void Start()
    {
        gameObject.GetComponent<EnemyMovement>().enabled = true;
        if (enemyCollider != null && stuckPrevention)
        {
            enemyCollider.isTrigger = false;
        }
        if (!stuckPrevention) enemyCollider.isTrigger = true;

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

        if (SO.enemySO.enemyType == EnemyType.Skelenton || SO.enemySO.enemyType == EnemyType.Demon || SO.enemySO.enemyType == EnemyType.BombGoblin)
        {
            if (attackingType == ChooseAttakingType.Tower)
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
            else if (attackingType == ChooseAttakingType.Core)
            {
                Collider2D coreCollider = Physics2D.OverlapCircle(transform.position, enemyShoot.detectionRange, enemyShoot.coreLayerMask);
                if (coreCollider != null)
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
        }

        if (!isSkeletonStopped)
        {
            FindTarget();
            if (stuckPrevention)
            {
                ObstacleFrontEnemy();
            }
            MoveToTarget();
        }
    }
    public void FindTarget()
    {
        GameObject closestTarget = null;

        LayerMask targetLayerMask = GetTargetLayerMask();

        if (targetLayerMask == 0)
        {
            Debug.LogWarning($"Invalid attacking type: {attackingType}");
            return;
        }
        
        if (useUnlimitedRange)
        {
            // ��ü ������ �˻�
            closestTarget = FindTargetInEntireScene(targetLayerMask);
        }
        else
        {
            // ���ѵ� �������� �˻�
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
            // ���̾� üũ
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
    /// Ư�� ���� �� Ÿ���� ã�� �Լ�
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
    public void MoveToTarget()
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

        transform.position += smoothedDirection * (SO.enemySO.enemySpeed + speedMagnification) * Time.deltaTime;
    }

    public Vector3 GetMovementDirection()
    {
        return smoothedDirection;
    }
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
    private void ObstacleFrontEnemy()
    {
        Vector3 forward = (target.transform.position - transform.position).normalized;

        // �۷ι� ����
        Vector3 rightDirection = Vector3.right;
        Vector3 leftDirection = Vector3.left;
        Vector3 upDirection = Vector3.up;
        Vector3 downDirection = Vector3.down;

        // ���� ����
        Vector3 localrightDirection = Vector3.Cross(Vector3.forward, forward).normalized;
        Vector3 localleftDirection = -localrightDirection;
        Vector3 localupDirection = forward;
        Vector3 localdownDirection = -forward;

        // �밢�� ����
        Vector3 diagonalUR = (localupDirection + localrightDirection).normalized; // ���
        Vector3 diagonalUL = (localupDirection + localleftDirection).normalized;  // �»�
        Vector3 diagonalDR = (localdownDirection + localrightDirection).normalized; // ����
        Vector3 diagonalDL = (localdownDirection + localleftDirection).normalized;  // ����

        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, forward, detectionDistance, obstacleLayerMask);

        // �۷ι�
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, rightDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, leftDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D upHit = Physics2D.Raycast(transform.position, upDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D downHit = Physics2D.Raycast(transform.position, downDirection, detectionDistance * 0.8f, obstacleLayerMask);
        
        // ����
        RaycastHit2D localrightHit = Physics2D.Raycast(transform.position, localrightDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D localleftHit = Physics2D.Raycast(transform.position, localleftDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D localupHit = Physics2D.Raycast(transform.position, localupDirection, detectionDistance * 0.8f, obstacleLayerMask);
        RaycastHit2D localdownHit = Physics2D.Raycast(transform.position, localdownDirection, detectionDistance * 0.8f, obstacleLayerMask);
        
        // �밢��
        RaycastHit2D diagonalURHit = Physics2D.Raycast(transform.position, diagonalUR, detectionDistance * 0.7f, obstacleLayerMask);
        RaycastHit2D diagonalULHit = Physics2D.Raycast(transform.position, diagonalUL, detectionDistance * 0.7f, obstacleLayerMask);
        RaycastHit2D diagonalDRHit = Physics2D.Raycast(transform.position, diagonalDR, detectionDistance * 0.7f, obstacleLayerMask);
        RaycastHit2D diagonalDLHit = Physics2D.Raycast(transform.position, diagonalDL, detectionDistance * 0.7f, obstacleLayerMask);

        Debug.DrawRay(transform.position, forward * detectionDistance, Color.red);
        Debug.DrawRay(transform.position, rightDirection * detectionDistance * 0.8f, Color.yellow);
        Debug.DrawRay(transform.position, leftDirection * detectionDistance * 0.8f, Color.yellow);
        Debug.DrawRay(transform.position, upDirection * detectionDistance * 0.8f, Color.blue);
        Debug.DrawRay(transform.position, downDirection * detectionDistance * 0.8f, Color.blue);

        Debug.DrawRay(transform.position, localrightDirection * detectionDistance * 0.8f, Color.green);
        Debug.DrawRay(transform.position, localleftDirection * detectionDistance * 0.8f, Color.green);

        Debug.DrawRay(transform.position, diagonalUR * detectionDistance * 0.7f, Color.cyan);
        Debug.DrawRay(transform.position, diagonalUL * detectionDistance * 0.7f, Color.cyan);
        Debug.DrawRay(transform.position, diagonalDR * detectionDistance * 0.7f, Color.magenta);
        Debug.DrawRay(transform.position, diagonalDL * detectionDistance * 0.7f, Color.magenta);

        Vector3 newAvoidDirection = Vector3.zero;

        // �տ� ��ֹ��� ���� ���� ȸ�� ���
        if (forwardHit.collider != null)
        {
            // ���⺰ ����ִ� ���� ã��
            bool rightClear = rightHit.collider == null;
            bool leftClear = leftHit.collider == null;
            bool upClear = upHit.collider == null;
            bool downClear = downHit.collider == null;

            bool localrightClear = localrightHit.collider == null;
            bool localleftClear = localleftHit.collider == null;
            bool localupClear = localupHit.collider == null;
            bool localdownClear = localdownHit.collider == null;

            bool diagonalURClear = diagonalURHit.collider == null;
            bool diagonalULClear = diagonalULHit.collider == null;
            bool diagonalDRClear = diagonalDRHit.collider == null;
            bool diagonalDLClear = diagonalDLHit.collider == null;

            // �켱����: ���� �¿�
            if (localrightClear && !localleftClear)
            {
                newAvoidDirection = Vector3.right;
            }
            else if (localleftClear && !localrightClear)
            {
                newAvoidDirection = Vector3.left;
            }
            else if (localrightClear && localleftClear)
            {
                newAvoidDirection = UnityEngine.Random.value > 0.5f ? localrightDirection : localleftDirection;
            }
            // �켱���� 3: �۷ι� �¿�
            if (diagonalURClear)
            {
                newAvoidDirection = diagonalUR;
            }
            else if (diagonalULClear)
            {
                newAvoidDirection = diagonalUL;
            }
            else if (diagonalDRClear)
            {
                newAvoidDirection = diagonalDR;
            }
            else if (diagonalDLClear)
            {
                newAvoidDirection = diagonalDL;
            }
            else if (rightClear && !leftClear)
            {
                newAvoidDirection = rightDirection;
            }
            else if (leftClear && !rightClear)
            {
                newAvoidDirection = leftDirection;
            }
            // �켱���� 4: �۷ι� ����
            else if (upClear)
            {
                newAvoidDirection = upDirection;
            }
            else if (downClear)
            {
                newAvoidDirection = downDirection;
            }
            else
            {
                // ��� ������ �������� �ڷ�
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
        /*//������ ���� �����ʸ� ����
        if (angle > 90 || angle < -90)
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
        // Ÿ�� �˻� ���� ǥ��
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