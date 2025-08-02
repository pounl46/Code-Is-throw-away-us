using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    private List<GameObject> _currentEnemyList = new List<GameObject>(); // 현재 웨이브에 사용할 적 목록

    private float _currentTime;
    private float _createTime;
    [SerializeField] private float _minTime = 0.5f;
    [SerializeField] private float _maxTime = 2.5f;

    [SerializeField] private Transform[] _spawnPoints;
    private GameObject[] _enemyPool;
    public int _enemyCount = 10;

    public bool isWave = true;
    public static EnemySpawnManager Instance { get; private set; }

    private int _currentWave = 1; // 현재 웨이브

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
    }

    public void StartWave(int waveNumber)
    {
        _currentWave = waveNumber;

        // 웨이브별 적 종류 설정 (1웨이브일 때는 1종류, 2웨이브는 2종류...)
        int enemyTypeCount = Mathf.Min(_enemyPrefab.Length, waveNumber);
        _currentEnemyList.Clear();
        for (int i = 0; i < enemyTypeCount; i++)
        {
            _currentEnemyList.Add(_enemyPrefab[i]);
        }

        // 웨이브가 오를수록 스폰 속도 빨라짐
        float speedUp = Mathf.Clamp(0.1f * (_currentWave - 1), 0, 1.5f);
        _minTime = Mathf.Max(0.2f, 0.5f - speedUp);
        _maxTime = Mathf.Max(0.5f, 2.5f - speedUp);

        _createTime = Random.Range(_minTime, _maxTime);

        isWave = true;
        Debug.Log($"Wave {_currentWave} 시작 - 적 종류: {enemyTypeCount}개, 스폰 간격: {_minTime} ~ {_maxTime}");
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
            // 웨이브가 끝나면 적 전부 비활성화
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
            if (!_enemyPool[i].activeSelf)
            {
                int index = Random.Range(0, _spawnPoints.Length);

                // 현재 웨이브에서 선택된 적 종류만 스폰
                GameObject enemyPrefab = _currentEnemyList[Random.Range(0, _currentEnemyList.Count)];
                _enemyPool[i] = Instantiate(enemyPrefab, transform);
                _enemyPool[i].SetActive(true);
                _enemyPool[i].transform.position = _spawnPoints[index].position;

                var coinSteal = _enemyPool[i].GetComponent<CoinSteal>();
                if (coinSteal != null)
                {
                    coinSteal.SetSpawnPoint(_spawnPoints[index]);
                }
                return;
            }
        }
    }

    private void CreateEnemyPool()
    {
        _enemyPool = new GameObject[_enemyCount];
        for (int i = 0; i < _enemyCount; i++)
        {
            _enemyPool[i] = Instantiate(_enemyPrefab[0], transform); // 초기엔 첫 번째 적으로 채움
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

