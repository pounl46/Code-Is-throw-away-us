using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween 네임스페이스 추가

// 포탑 배치 시스템의 핵심 매니저
public class TowerGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 8;
    public int gridHeight = 8;

    private int[,] gameGrid;

    public bool easyLimit = false;
    public bool hardLimit = false;

    bool canMove = false;

    [Header("Player Control")]
    public int currentX = 0;
    public int currentY = 0;

    [Header("Animation Settings")]
    public float moveDuration = 0.2f;  // 움직임 시간
    public Ease moveEase = Ease.OutQuad; // 이징 타입

    private bool isMoving = false; // 움직임 중인지 체크

    [Header("Prefabs")]
    public GameObject towerPrefab;
    public GameObject cursorPrefab;

    private GameObject cursorObject;

    void Start()
    {
        InitializeGrid();
        CreateCursor();
    }

    void InitializeGrid()
    {
        gameGrid = new int[gridWidth, gridHeight];

        // 모든 칸을 빈 공간(0)으로 초기화
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gameGrid[x, y] = 0;
            }
        }
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // 움직임 중이면 입력 무시 (연타 방지)
        if (isMoving) return;

        // 방향키 입력
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayer(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayer(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovePlayer(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MovePlayer(0, -1);
        }

        // 스페이스바로 포탑 설치
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryPlaceTower();
        }
    }

    void MovePlayer(int deltaX, int deltaY)
    {
        int newX = currentX + deltaX;
        int newY = currentY + deltaY;

        

        // 경계 체크
        if (!easyLimit && !hardLimit)
        {
            if (newX >= 0 && newX < gridWidth && newY >= 0 && newY < gridHeight)
            {
                canMove = true;
            }
        }
        else if (easyLimit && !hardLimit)
        {
            if (newX >= 1 && newX < gridWidth - 1 && newY >= 1 && newY < gridHeight - 1)
            {
                canMove = true;
            }
        }
        else if (hardLimit)
        {
            if (newX >= 2 && newX < gridWidth - 2 && newY >= 2 && newY < gridHeight - 2)
            {
                canMove = true;
            }
        }

        // 움직일 수 있으면 애니메이션으로 이동
        if (canMove)
        {
            currentX = newX;
            currentY = newY;
            AnimateToPosition();
        }
    }

    void AnimateToPosition()
    {
        
        if (cursorObject == null) return;

        isMoving = true;
        Vector3 targetPosition = GridToWorldPosition(currentX, currentY);

        // DOTween으로 부드럽게 이동
        cursorObject.transform.DOMove(targetPosition, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() => {
                isMoving = false;
                UpdateCursorColor(); // 이동 완료 후 색상 업데이트
            });
    }

    void TryPlaceTower()
    {
        // 현재 위치에 이미 무언가 있는지 체크
        if (CanPlaceTower(currentX, currentY))
        {
            PlaceTower(currentX, currentY);
            Debug.Log($"포탑 설치 완료: ({currentX}, {currentY})");
            UpdateCursorColor(); // 포탑 설치 후 커서 색상 업데이트
        }
        else
        {
            Debug.Log($"포탑 설치 불가: ({currentX}, {currentY}) - 이미 오브젝트가 있습니다!");

        }
    }

    bool CanPlaceTower(int x, int y)
    {
        // 경계 체크
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return false;

        // 해당 위치가 비어있는지 체크 (0이면 빈 공간)
        return gameGrid[x, y] == 0;
    }

    void PlaceTower(int x, int y)
    {
        // 배열에 포탑 표시 (1)
        gameGrid[x, y] = 1;

        // 실제 게임 오브젝트 생성
        Vector3 worldPosition = GridToWorldPosition(x, y);
        GameObject tower = Instantiate(towerPrefab, worldPosition, Quaternion.identity);

        // 포탑에 좌표 정보 저장 (나중에 삭제할 때 사용)
        Tower towerScript = tower.GetComponent<Tower>();
        if (towerScript != null)
        {
            towerScript.gridX = x;
            towerScript.gridY = y;
        }
    }

    Vector3 GridToWorldPosition(int x, int y)
    {
        // 그리드 좌표를 월드 좌표로 변환
        // 타일 크기를 1로 가정
        return new Vector3(x, y, 0);
    }

    void CreateCursor()
    {
        if (cursorPrefab != null)
        {
            cursorObject = Instantiate(cursorPrefab);
            cursorObject.transform.position = GridToWorldPosition(currentX, currentY);
            UpdateCursorColor();
        }
    }

    void UpdateCursorColor()
    {
        if (cursorObject != null)
        {
            // 현재 위치에 설치 가능한지에 따라 커서 색상 변경
            Renderer cursorRenderer = cursorObject.GetComponent<Renderer>();
            if (cursorRenderer != null)
            {
                if (CanPlaceTower(currentX, currentY))
                {
                    cursorRenderer.material.color = Color.green; // 설치 가능
                }
                else
                {
                    cursorRenderer.material.color = Color.red;   // 설치 불가
                }
            }
        }
    }
}