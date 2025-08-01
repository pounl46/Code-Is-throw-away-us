using TMPro;
using UnityEngine;

public class Information : MonoBehaviour
{
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text str;
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text delay;
    [SerializeField] private GameObject panel;
    [SerializeField] private LayerMask layerMask;

    private TowerHealthManager currentHealthManager;
    private TowerSetting currentTowerSetting;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, layerMask);

            if (hit.collider != null)
            {
                currentHealthManager = hit.collider.GetComponent<TowerHealthManager>();
                currentTowerSetting = hit.collider.GetComponent<TowerSetting>();

                if (currentHealthManager != null && currentTowerSetting != null)
                {
                    panel.SetActive(true);
                    towerName.text = hit.collider.name;
                    str.text = "Str : " + currentTowerSetting.attackDamage.ToString();
                    delay.text = "delay : " + currentTowerSetting.attackDelay.ToString() + "/s";
                }
            }
        }

        // 실시간 hp 갱신
        if (panel.activeSelf && currentHealthManager != null && currentTowerSetting != null)
        {
            hp.text = "Hp : " + currentHealthManager.nowTowerHealth + " / " + currentTowerSetting.Health;
        }
    }

    public void CloseStat()
    {
        panel.SetActive(false);
        currentHealthManager = null;
        currentTowerSetting = null;
    }
}
