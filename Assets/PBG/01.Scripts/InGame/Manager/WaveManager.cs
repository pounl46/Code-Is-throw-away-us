using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEditor.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public float restTime = 10f; // 쉬는 시간
    private bool isResting = false;
    [SerializeField] private bool isWaveActive = false;
    private int currentWave = 0;
    private float waveTime = 30;

    private float wavTime = 0;

    public Button skipButton;
    public TMP_Text waveText;
    public TMP_Text timerText;

    private Coroutine restCoroutine;
    public EnemySpawnManager _enemySpawnManager;

    void Start()
    {
        _enemySpawnManager = GetComponent<EnemySpawnManager>();
        skipButton.onClick.AddListener(SkipRestTime);
        StartNextWave();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EndWave();
        }
    }

    public void StartNextWave()
    {
        currentWave++;
        isWaveActive = true;
        isResting = false;
        waveText.text = $"Wave {currentWave}";
        timerText.text = "";
        waveTime = 30;
        skipButton.gameObject.SetActive(false);

        // 웨이브 정보 전달
        _enemySpawnManager.StartWave(currentWave);
    
        if (currentWave > 1)
        {
            wavTime += 10;
            waveTime = 30 + wavTime;
        }
    
        Debug.Log("웨이브 시작: " + currentWave);
        StartCoroutine(WaveTimer());
    }


    public void EndWave()
    {
        if (!isWaveActive) return;

        isWaveActive = false;
        Debug.Log("웨이브 종료");
        _enemySpawnManager.isWave = false;

        StartRestTime();
    }

    void StartRestTime()
    {
        isResting = true;
        skipButton.gameObject.SetActive(true);
        restCoroutine = StartCoroutine(RestTimer());
    }

    IEnumerator RestTimer()
    {
        float remainingTime = restTime;

        while (remainingTime > 0)
        {
            timerText.text = $"Next Wave: {remainingTime:F1}s";
            yield return new WaitForSeconds(0.1f);
            remainingTime -= 0.1f;
        }

        timerText.text = "";
        skipButton.gameObject.SetActive(false);
        StartNextWave();
    }

    private IEnumerator WaveTimer()
    {

        while (waveTime > 0)
        {
            timerText.text = $"Now Wave: {waveTime:F1}s";
            yield return new WaitForSeconds(0.1f);
            waveTime -= 0.1f;
        }

        timerText.text = "";
        EndWave();
    }

    void SkipRestTime()
    {
        if (!isResting) return;

        Debug.Log("쉬는 시간 스킵됨");
        if (restCoroutine != null)
        {
            StopCoroutine(restCoroutine);
        }

        timerText.text = "";
        skipButton.gameObject.SetActive(false);
        StartNextWave();
    }
}
