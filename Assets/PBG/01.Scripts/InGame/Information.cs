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
    [SerializeField] private GameObject deleyParent;

    private TowerHealthManager currentHealthManager;
    private TowerSetting currentTowerSetting;
    private MoneyTower currentMoneyTower;

    private GameObject currentSelectedTower;

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
                currentSelectedTower = hit.collider.gameObject;

                currentHealthManager = hit.collider.GetComponent<TowerHealthManager>();
                currentTowerSetting = hit.collider.GetComponent<TowerSetting>();
                currentMoneyTower = hit.collider.GetComponent<MoneyTower>();
                string objName = hit.collider.name;
                string cleanName = objName.Replace("(Clone)", "");

                Debug.Log(1);

                if (currentHealthManager != null && currentTowerSetting != null)
                {
                    panel.SetActive(true);
                    panel.transform.position = Input.mousePosition;
                    // deleyParent.SetActive(true);
                    towerName.text = cleanName;
                    str.text = "Str : " + currentTowerSetting.attackDamage.ToString();
                    delay.text = "delay : " + currentTowerSetting.attackDelay.ToString() + "/s";
                }


                else if (currentMoneyTower != null)
                {
                    panel.SetActive(true);
                    panel.transform.position = Input.mousePosition;
                    towerName.text = cleanName;
                    hp.text = "Money : " + currentMoneyTower.TowerSO.Money.ToString();
                    deleyParent.SetActive(false);
                }
            }
        }

        // 실시간 hp 갱신
        if (panel.activeSelf && currentHealthManager != null && currentTowerSetting != null)
        {
            hp.text = "Hp : " + currentHealthManager.nowTowerHealth + " / " + currentTowerSetting.Health;
        }

        else if (panel.activeSelf && currentMoneyTower != null)
        {
            str.text = "Delay : " + (currentMoneyTower.TowerSO.Money / currentMoneyTower.TowerSO.WaitTime).ToString() + "/s";
        }
    }

    public void CloseStat()
    {
        panel.SetActive(false);
        currentHealthManager = null;
        currentTowerSetting = null;
        currentMoneyTower = null;
        currentSelectedTower = null;
    }
    public void SellTower() // UI 버튼에서 호출할 메서드
    {
        if (currentSelectedTower != null && TowerGridManager.Instance != null)
        {
            Tower tower = currentSelectedTower.GetComponent<Tower>();
            if (tower != null)
            {
                Vector2Int gridPos = new Vector2Int(tower.gridX, tower.gridY);
                TowerGridManager.Instance.SellTowerAtPosition(gridPos);
                CloseStat();
                Debug.Log($"포탑 판매 완료: {currentSelectedTower.name} at ({gridPos.x}, {gridPos.y})");
            }
        }
    }
}