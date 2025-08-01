using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBarManager : MonoBehaviour
{
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private Canvas hpBarCanvas;
    [SerializeField] private Camera mainCamera;

    private Dictionary<Enemy, GameObject> hpBarInstances = new Dictionary<Enemy, GameObject>();

    public static EnemyHpBarManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        // 이미 존재하는 모든 적들에게 HP바 추가
        Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in existingEnemies)
        {
            AddHpBarToEnemy(enemy);
        }
    }

    public void AddHpBarToEnemy(Enemy enemy)
    {
        if (enemy == null || hpBarInstances.ContainsKey(enemy)) return;

        // HP바 인스턴스 생성
        GameObject hpBarObj = Instantiate(hpBarPrefab, hpBarCanvas.transform);

        // WorldToScreenController 설정
        WorldToScreenController controller = hpBarObj.GetComponent<WorldToScreenController>();
        if (controller == null)
        {
            controller = hpBarObj.AddComponent<WorldToScreenController>();
        }

        // HpBarView 설정
        HpBarView hpView = hpBarObj.GetComponent<HpBarView>();
        if (hpView == null)
        {
            hpView = hpBarObj.AddComponent<HpBarView>();
        }

        // 컨트롤러에 참조 연결
        controller.hpView = hpView;
        controller.enemy = enemy;
        controller.worldCanvas = null; // Screen Space 사용

        // 딕셔너리에 저장
        hpBarInstances[enemy] = hpBarObj;
    }

    public void RemoveHpBarFromEnemy(Enemy enemy)
    {
        if (hpBarInstances.TryGetValue(enemy, out GameObject hpBarObj))
        {
            if (hpBarObj != null)
            {
                Destroy(hpBarObj);
            }
            hpBarInstances.Remove(enemy);
            Debug.Log($"HP Bar removed from enemy: {enemy.name}");
        }
    }

    private void Update()
    {
        // 죽은 적들의 HP바 정리
        List<Enemy> enemiesToRemove = new List<Enemy>();
        foreach (var kvp in hpBarInstances)
        {
            if (kvp.Key == null || !kvp.Key.IsAlive)
            {
                enemiesToRemove.Add(kvp.Key);
            }
        }

        foreach (Enemy enemy in enemiesToRemove)
        {
            RemoveHpBarFromEnemy(enemy);
        }
    }
}
