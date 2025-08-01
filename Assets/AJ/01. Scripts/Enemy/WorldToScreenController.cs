using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class WorldToScreenController : MonoBehaviour
{
    public HpBarView hpView;
    public Enemy enemy;
    private Camera mainCamera;
    public Canvas worldCanvas;
    private HealthSystem healthSystem;

    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (worldCanvas == null)
            worldCanvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        if (enemy != null)
        {
            healthSystem = enemy.GetComponent<HealthSystem>();

            if (healthSystem != null)
            {
                healthSystem.OnDamage += UpdateHpBar;
                healthSystem.OnDead += HideHpBar;
            }
        }

        if (hpView != null)
        {
            offset = new Vector3(0, hpView.steeringValue, 0);
            UpdateHpBar();
        }
    }

    private void Update()
    {
        if (enemy != null && enemy.IsAlive && hpView != null && hpView.gameObject.activeSelf)
        {
            UpdateHpBarPosition();
        }
    }

    private void UpdateHpBarPosition()
    {
        if (enemy == null || hpView == null) return;

        // 적의 위치에서 오프셋만큼 위로 이동
        Vector3 worldPosition = enemy.transform.position + offset;

        if (worldCanvas != null && worldCanvas.renderMode == RenderMode.WorldSpace)
        {
            hpView.SetPosition(worldPosition);
        }
        else
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
            hpView.SetPosition(screenPos);
        }
    }

    private void UpdateHpBar()
    {
        if (enemy != null && hpView != null)
        {
            float hpRatio = enemy.HpRatio;
            hpView.SetSliderRatio(hpRatio);

            if (hpRatio <= 1f && hpRatio > 0f)
            {
                hpView.gameObject.SetActive(true);
            }
            else
            {
                hpView.gameObject.SetActive(false);
            }
        }
    }

    private void HideHpBar()
    {
        if (hpView != null)
            hpView.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnDamage -= UpdateHpBar;
            healthSystem.OnDead -= HideHpBar;
        }
    }
}
