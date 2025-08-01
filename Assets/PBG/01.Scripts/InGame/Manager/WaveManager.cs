using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public float restTime = 10f; // 쉬는 시간
    private bool isResting = false;
    [SerializeField] private bool isWaveActive = false;
    private int currentWave = 0;

    public Button skipButton;
    public TMP_Text waveText;
    public TMP_Text timerText;

    private Coroutine restCoroutine;

    void Start()
    {
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
        skipButton.gameObject.SetActive(false);

        Debug.Log("웨이브 시작: " + currentWave);
    }

    public void EndWave()
    {
        if (!isWaveActive) return;

        isWaveActive = false;
        Debug.Log("웨이브 종료");

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
