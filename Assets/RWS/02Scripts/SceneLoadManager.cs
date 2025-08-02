using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }
    [SerializeField] private GameObject fadeCanvas;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Transform panelTarget1;
    [SerializeField] private Transform panelTarget2;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(fadeCanvas);
            DontDestroyOnLoad(fadePanel);
            DontDestroyOnLoad(panelTarget1.gameObject);
            DontDestroyOnLoad(panelTarget2.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartFade(Action onComplete = null)
    {
        fadePanel.transform.DOMove(panelTarget1.position, 3f).SetEase(Ease.InExpo)
            .OnComplete(() => {
            onComplete?.Invoke();
        });
    }

    public void EndFade()
    {
        fadePanel.transform.DOMove(panelTarget2.position, 3f).SetEase(Ease.OutExpo).SetDelay(1f);
    }

    public void LoadScene(int index)
    {
        StartFade(() => {
            SceneManager.LoadScene(index);
        });
    }

    public void LoadScene0()
    {
        StartFade(() => {
            SceneManager.LoadScene(0);
        });
    }

    public void LoadScene1()
    {
        StartFade(() => {
            SceneManager.LoadScene(1);
        });
    }

    public void LoadScene2()
    {
        StartFade(() => {
            SceneManager.LoadScene(2);
        });
    }

    public void LoadScene3()
    {
        StartFade(() => {
            SceneManager.LoadScene(3);
        });
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        EndFade();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
