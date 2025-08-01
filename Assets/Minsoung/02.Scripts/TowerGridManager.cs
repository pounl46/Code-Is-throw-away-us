using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween 네임스페이스 추가
using System.Collections;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using Unity.Collections;

// 포탑 배치 시스템의 핵심 매니저
public class TowerGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 8;
    public int gridHeight = 8;

    private int[,] gameGrid;

    public bool easyLimit = false;
    public bool hardLimit = false;

    [SerializeField] private Camera _camera;
    [SerializeField] private Tilemap _boardMap;
    [SerializeField] private Tile _board1;
    [SerializeField] private Tile _board2;

    bool canMove = false;

    // 포탑 관리를 위한 딕셔너리 추가
    private Dictionary<Vector2Int, GameObject> placedTowers = new Dictionary<Vector2Int, GameObject>();

    [Header("Player Control")]
    public int currentX = 3;
    public int currentY = 5;

    [Header("Animation Settings")]
    public float moveDuration = 0.2f;  // 움직임 시간
    public Ease moveEase = Ease.OutQuad; // 이징 타입

    private bool isMoving = false; // 움직임 중인지 체크

    [Header("Prefabs")]
    public GameObject towerPrefab;
    public GameObject cursorPrefab;

    private GameObject cursorObject;

    // 싱글톤 패턴 추가 (Information 클래스에서 접근하기 위해)
    public static TowerGridManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _camera = Camera.main;
        InitializeGrid();
        cursorObject = Instantiate(cursorPrefab);
        cursorObject.SetActive(false);
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
        gameGrid[3, 3] = 1;
        gameGrid[3, 4] = 1;
        gameGrid[4, 3] = 1;
        gameGrid[4, 4] = 1;
    }

    void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.T))
        {
            EasyLimit();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            HardLimit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            EasyCreate();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            HardCreate();
        }
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
                currentX = newX;
                currentY = newY;
            }
        }
        else if (easyLimit && !hardLimit)
        {
            if (newX >= 1 && newX < gridWidth - 1 && newY >= 1 && newY < gridHeight - 1)
            {
                canMove = true;
                currentX = newX;
                currentY = newY;
            }
        }
        else if (hardLimit)
        {
            if (newX >= 2 && newX < gridWidth - 2 && newY >= 2 && newY < gridHeight - 2)
            {
                canMove = true;
                currentX = newX;
                currentY = newY;
            }
        }

        // 움직일 수 있으면 애니메이션으로 이동
        if (canMove)
        {
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
        if (cursorObject.activeSelf)
        {
            // 현재 위치에 이미 무언가 있는지 체크
            if (CanPlaceTower(currentX, currentY))
            {
                PlaceTower(currentX, currentY);
                Debug.Log($"포탑 설치 완료: ({currentX}, {currentY})");
                UpdateCursorColor(); // 포탑 설치 후 커서 색상 업데이트
                cursorObject.SetActive(false);
            }
            else
            {
                _camera.DOShakePosition(1f, 0.1f, 5, 90f, true);
                Debug.Log($"포탑 설치 불가: ({currentX}, {currentY}) - 이미 오브젝트가 있습니다!");
            }
        }
        else
        {
            Debug.Log("커서 비활성화임");
        }
    }

    // Information 클래스에서 호출할 수 있는 공개 메서드
    public void SellTowerAtPosition(Vector2Int gridPos)
    {
        SellTower(gridPos.x, gridPos.y);
    }

    // 월드 좌표에서 그리드 좌표로 변환하는 메서드 추가
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.y);
        return new Vector2Int(x, y);
    }

    void SellTower(int x, int y)
    {
        Vector2Int gridPos = new Vector2Int(x, y);

        if (placedTowers.ContainsKey(gridPos))
        {
            // 게임 오브젝트 삭제
            Destroy(placedTowers[gridPos]);

            // 딕셔너리에서 제거
            placedTowers.Remove(gridPos);

            // 그리드 상태 업데이트
            gameGrid[x, y] = 0;

            Debug.Log($"포탑 판매됨: ({x}, {y})");
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

        // 딕셔너리에 포탑 추가
        Vector2Int gridPos = new Vector2Int(x, y);
        placedTowers[gridPos] = tower;

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
        return new Vector3(x + 0.5f, y + 0.5f, 0);
    }

    public void CreateCursor()
    {
        if (cursorPrefab != null)
        {
            if (cursorObject.activeSelf == false)
            {
                cursorObject.SetActive(true);
                currentX = 3;
                currentY = 5;
                cursorObject.transform.position = GridToWorldPosition(currentX, currentY);
                UpdateCursorColor();
            }
        }
    }

    void UpdateCursorColor()
    {
        if (cursorObject != null)
        {
            Renderer cursorRenderer = cursorObject.GetComponent<Renderer>();
            if (cursorRenderer != null)
            {
                // 설치 모드만 남김
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

    // 제한 범위 밖의 포탑들을 삭제하는 메서드
    void RemoveTowersOutsideBounds()
    {
        List<Vector2Int> towersToRemove = new List<Vector2Int>();

        foreach (var kvp in placedTowers)
        {
            Vector2Int gridPos = kvp.Key;
            int x = gridPos.x;
            int y = gridPos.y;

            bool shouldRemove = false;

            if (easyLimit && !hardLimit)
            {
                // Easy 제한: 가장자리 1줄 밖의 포탑 제거
                if (x <= 0 || x >= gridWidth - 1 || y <= 0 || y >= gridHeight - 1)
                {
                    shouldRemove = true;
                }
            }
            else if (hardLimit)
            {
                // Hard 제한: 가장자리 2줄 밖의 포탑 제거
                if (x <= 1 || x >= gridWidth - 2 || y <= 1 || y >= gridHeight - 2)
                {
                    shouldRemove = true;
                }
            }

            if (shouldRemove)
            {
                towersToRemove.Add(gridPos);
            }
        }

        // 제거할 포탑들 삭제
        foreach (Vector2Int gridPos in towersToRemove)
        {
            SellTower(gridPos.x, gridPos.y);
            Debug.Log($"제한 범위 밖 포탑 자동 삭제: ({gridPos.x}, {gridPos.y})");
        }
    }

    public void EasyLimit()
    {
        easyLimit = true;

        // 제한 범위 밖의 포탑들 삭제
        RemoveTowersOutsideBounds();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (x == 0 || x == 7 || y == 0 || y == 7)
                {
                    _boardMap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    public void HardLimit()
    {
        hardLimit = true;

        // 제한 범위 밖의 포탑들 삭제
        RemoveTowersOutsideBounds();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (x == 1 || x == 6 || y == 1 || y == 6 || x == 0 || x == 7 || y == 0 || y == 7)
                {
                    _boardMap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    public void EasyCreate()
    {
        easyLimit = false; // 제한 해제

        // 경계 부분 타일들을 체스판 패턴으로 재생성
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (x == 0 || x == 7 || y == 0 || y == 7)
                {
                    // 체스판 패턴 결정 (x + y가 짝수면 _board1, 홀수면 _board2)
                    Tile tileToUse = (x + y) % 2 == 0 ? _board1 : _board2;
                    _boardMap.SetTile(new Vector3Int(x, y, 0), tileToUse);
                }
            }
        }
    }

    public void HardCreate()
    {
        hardLimit = false; // 제한 해제

        // 안쪽 경계 부분 타일들만 체스판 패턴으로 재생성 (인덱스 1, 6만)
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                // 안쪽 테두리만 복원 (바깥쪽 경계 0,7은 제외하고 안쪽 경계 1,6만)
                if ((x == 1 || x == 6) && y >= 1 && y <= 6 ||
                    (y == 1 || y == 6) && x >= 1 && x <= 6)
                {
                    // 체스판 패턴 결정 (x + y가 짝수면 _board1, 홀수면 _board2)
                    Tile tileToUse = (x + y) % 2 == 0 ? _board1 : _board2;
                    _boardMap.SetTile(new Vector3Int(x, y, 0), tileToUse);
                }
            }
        }
    }
}