using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    private float _currentTime;
    private float _createTime;
    [SerializeField] private float _minTime = 0.5f;
    [SerializeField] private float _maxTime = 2.5f;
    [SerializeField] private Transform[] _spawnPoints;
    private GameObject[] _enemyPool;
    public int _enemyCount = 10;

    public bool isWave = true;

    public static EnemySpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CreateEnemyPool();
        _createTime = Random.Range(_minTime, _maxTime);
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (isWave)
        {
            if (_currentTime >= _createTime)
            {
                SpawnEnemyFromPool();
                _currentTime = 0;
                _createTime = Random.Range(_minTime, _maxTime);
            }
        }
        else
        {
            for (int i = 0; i < _enemyCount; i++)
        {
            if (_enemyPool[i].activeSelf)
            {
                _enemyPool[i].SetActive(false);
            }
        }
        }
    }

    private void SpawnEnemyFromPool()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            if (_enemyPool[i] == null)
            {
                _enemyPool[i] = Instantiate(_enemyPrefab[Random.Range(0, _enemyPrefab.Length)], transform);
                _enemyPool[i].SetActive(false);

                // 새로 생성된 적에게 체력바 추가
                Enemy enemy = _enemyPool[i].GetComponent<Enemy>();
                if (enemy != null && EnemyHpBarManager.Instance != null)
                {
                    EnemyHpBarManager.Instance.AddHpBarToEnemy(enemy);
                }
            }

            if (!_enemyPool[i].activeSelf)
            {
                int index = Random.Range(0, _spawnPoints.Length);
                _enemyPool[i].SetActive(true);
                _enemyPool[i].transform.position = _spawnPoints[index].position;

                var coinSteal = _enemyPool[i].GetComponent<CoinSteal>();
                if (coinSteal != null)
                {
                    coinSteal.SetSpawnPoint(_spawnPoints[index]);
                }
                break;
            }
        }

    }

    private void CreateEnemyPool()
    {
        _enemyPool = new GameObject[_enemyCount];
        for (int i = 0; i < _enemyCount; i++)
        {
            _enemyPool[i] = Instantiate(_enemyPrefab[Random.Range(0, _enemyPrefab.Length)], transform);

            Enemy enemy = _enemyPool[i].GetComponent<Enemy>();
            if (enemy == null && EnemyHpBarManager.Instance == null)
            {
                EnemyHpBarManager.Instance.AddHpBarToEnemy(enemy);
            }

            _enemyPool[i].SetActive(false);
        }
    }
    private void LateUpdate()
    {
        CheckAndReplenishPool();
    }

    private void CheckAndReplenishPool()
    {
        for (int i = 0; i < _enemyPool.Length; i++)
        {
            if (_enemyPool[i] == null)
            {
                _enemyPool[i] = Instantiate(_enemyPrefab[Random.Range(0, _enemyPrefab.Length)], transform);
                Enemy enemy = _enemyPool[i].GetComponent<Enemy>();
                if (enemy != null && EnemyHpBarManager.Instance != null)
                {
                    EnemyHpBarManager.Instance.AddHpBarToEnemy(enemy);
                }
                _enemyPool[i].SetActive(false);
            }
        }
    }


    // 전체 풀을 재생성
    public void RegenerateAllPool()
    {
        for (int i = 0; i < _enemyPool.Length; i++)
        {
            if (_enemyPool[i] != null)
            {
                Destroy(_enemyPool[i]);
            }
            _enemyPool[i] = Instantiate(_enemyPrefab[Random.Range(0, _enemyPrefab.Length)], transform);
            Enemy enemy = _enemyPool[i].GetComponent<Enemy>();
            if (enemy != null && EnemyHpBarManager.Instance != null)
            {
                EnemyHpBarManager.Instance.AddHpBarToEnemy(enemy);
            }
            _enemyPool[i].SetActive(false);
        }
    }
    public void DestroyAllPool()
    {
        for (int i = 0; i < _enemyPool.Length; i++)
        {
            if (_enemyPool[i] != null)
            {
                Destroy(_enemyPool[i]);
            }
        }
    }
}
