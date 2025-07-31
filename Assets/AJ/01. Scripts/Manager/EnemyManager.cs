using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    private float _currentTime;
    private float _createTime;
    [SerializeField] private float _minTime = 0.5f;
    [SerializeField] private float _maxTime = 2.5f;
    [SerializeField] private Transform[] _spawnPoints;
    private GameObject[] _enemyPool;
    public int _enemyCount = 10;

    public static EnemyManager Instance { get; private set; }

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
        if (_currentTime >= _createTime)
        {
            SpawnEnemyFromPool();
            _currentTime = 0;
            _createTime = Random.Range(_minTime, _maxTime);
        }
    }

    private void SpawnEnemyFromPool()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            if (!_enemyPool[i].activeSelf)
            {
                _enemyPool[i].SetActive(true);
                _enemyPool[i].transform.position = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
                break;
            }
        }
    }

    private void CreateEnemyPool()
    {
        _enemyPool = new GameObject[_enemyCount];
        for (int i = 0; i < _enemyCount; i++)
        {
            _enemyPool[i] = Instantiate(_enemyPrefab, transform);
            _enemyPool[i].SetActive(false);
        }
    }
}
