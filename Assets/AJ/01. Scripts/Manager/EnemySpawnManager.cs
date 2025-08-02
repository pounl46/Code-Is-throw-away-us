using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab; // 모든 몬스터 종류
    private List<GameObject> _currentEnemyList = new List<GameObject>(); // 현재 웨이브에서 스폰될 몬스터 목록

    private float _currentTime;
    private float _createTime;
    [SerializeField] private float _minTime = 0.5f;
    [SerializeField] private float _maxTime = 2.5f;

    [SerializeField] private Transform[] _spawnPoints;
    private GameObject[] _enemyPool; // 풀
    public int _enemyCount = 10; // 풀 크기

    public bool isWave = true;
    public static EnemySpawnManager Instance { get; private set; }

    private int _currentWave = 1; // 현재 웨이브

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CreateEnemyPool();
    }

    // 웨이브 시작 시 호출
    public void StartWave(int waveNumber)
    {
        _currentWave = waveNumber;
        _currentEnemyList.Clear();

        if (_currentWave < 11)
        {
            // 10웨이브 이전 → 한 종류만 (순환)
            int index = (_currentWave - 1) % _enemyPrefab.Length;
            _currentEnemyList.Add(_enemyPrefab[index]);
        }
        else
        {
            // 10웨이브 이상 → 매 웨이브마다 1종씩 추가 (최대 프리팹 수까지)
            int typeCount = Mathf.Min(_enemyPrefab.Length, _currentWave - 9);
            for (int i = 0; i < typeCount; i++)
            {
                _currentEnemyList.Add(_enemyPrefab[i]);
            }
        }

        // 웨이브 시작 시 모든 풀 오브젝트 비활성화
        for (int i = 0; i < _enemyCount; i++)
        {
            _enemyPool[i].SetActive(false);
        }

        // 웨이브가 오를수록 스폰 속도 빨라짐
        float speedUp = Mathf.Clamp(0.1f * (_currentWave - 1), 0, 1.5f);
        _minTime = Mathf.Max(0.2f, 0.5f - speedUp);
        _maxTime = Mathf.Max(0.5f, 5f - speedUp);

        _createTime = Random.Range(_minTime, _maxTime);

        isWave = true;
        Debug.Log($"Wave {_currentWave} 시작 - 몬스터 종류 { _currentEnemyList.Count}개, 스폰 간격: {_minTime} ~ {_maxTime}");
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
        // else
        // {
        //     // 웨이브 종료 시 모든 몬스터 비활성화
        //     for (int i = 0; i < _enemyCount; i++)
        //     {
        //         _enemyPool[i].SetActive(false);
        //     }
        // }
    }

    private void SpawnEnemyFromPool()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            if (!_enemyPool[i].activeSelf)
            {
                int spawnIndex = Random.Range(0, _spawnPoints.Length);

                // 현재 웨이브에서 사용할 몬스터 종류 중 랜덤 선택
                GameObject prefab = _currentEnemyList[Random.Range(0, _currentEnemyList.Count)];

                // 풀 안 오브젝트 교체 (Destroy 없이 교체)
                if (_enemyPool[i].name.Replace("(Clone)", "") != prefab.name)
                {
                    Destroy(_enemyPool[i]);
                    _enemyPool[i] = Instantiate(prefab, transform);
                    _enemyPool[i].SetActive(false);
                }

                _enemyPool[i].transform.position = _spawnPoints[spawnIndex].position;
                _enemyPool[i].SetActive(true);

                // CoinSteal 세팅
                var coinSteal = _enemyPool[i].GetComponent<CoinSteal>();
                if (coinSteal != null)
                    coinSteal.SetSpawnPoint(_spawnPoints[spawnIndex]);

                return;
            }
        }
    }

    private void CreateEnemyPool()
    {
        _enemyPool = new GameObject[_enemyCount];
        for (int i = 0; i < _enemyCount; i++)
        {
            _enemyPool[i] = Instantiate(_enemyPrefab[0], transform); // 기본 몬스터로 채움
            _enemyPool[i].SetActive(false);
        }
    }
}
