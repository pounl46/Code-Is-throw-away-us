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
        // �̹� �����ϴ� ��� ���鿡�� HP�� �߰�
        Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in existingEnemies)
        {
            AddHpBarToEnemy(enemy);
        }
    }

    public void AddHpBarToEnemy(Enemy enemy)
    {
        if (enemy == null || hpBarInstances.ContainsKey(enemy)) return;

        // HP�� �ν��Ͻ� ����
        GameObject hpBarObj = Instantiate(hpBarPrefab, hpBarCanvas.transform);

        // WorldToScreenController ����
        WorldToScreenController controller = hpBarObj.GetComponent<WorldToScreenController>();
        if (controller == null)
        {
            controller = hpBarObj.AddComponent<WorldToScreenController>();
        }

        // HpBarView ����
        HpBarView hpView = hpBarObj.GetComponent<HpBarView>();
        if (hpView == null)
        {
            hpView = hpBarObj.AddComponent<HpBarView>();
        }

        // ��Ʈ�ѷ��� ���� ����
        controller.hpView = hpView;
        controller.enemy = enemy;
        controller.worldCanvas = null; // Screen Space ���

        // ��ųʸ��� ����
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
        // ���� ������ HP�� ����
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
