using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Information : MonoBehaviour
{
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text str;
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text delay;
    [SerializeField] private GameObject panel;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject deleyParent;
    [SerializeField] private Button UpgradeBTN;

    private TowerHealthManager currentHealthManager;
    private TowerSetting currentTowerSetting;
    private MoneyTower currentMoneyTower;
    
    private GameObject currentSelectedTower;

    private GameObject SaveNowObj;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (panel.activeSelf && SaveNowObj != null)
        {
            UpgradeBTN.onClick.AddListener(() =>
            {
                if (SaveNowObj.GetComponent<TowerSetting>())
                {
                    SaveNowObj.GetComponent<UpgradeTower>().OnUpgrde();
                }
            });
        }
        if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, layerMask);
                if (hit.collider != null)
                {
                SaveNowObj = hit.collider.gameObject;
                    currentSelectedTower = hit.collider.gameObject;

                    currentHealthManager = hit.collider.GetComponent<TowerHealthManager>();
                    currentTowerSetting = hit.collider.GetComponent<TowerSetting>();
                    currentMoneyTower = hit.collider.GetComponent<MoneyTower>();
                    string objName = hit.collider.name;
                    string cleanName = objName.Replace("(Clone)", "");
                    if (currentHealthManager != null && currentTowerSetting != null)
                    {
                        panel.SetActive(true);
                        // deleyParent.SetActive(true);
                        towerName.text = cleanName;
                        str.text = currentTowerSetting.attackDamage.ToString();
                        delay.text = currentTowerSetting.attackDelay.ToString() + "/s";
                    }

                    else if (currentMoneyTower != null)
                    {
                        panel.SetActive(true);
                        towerName.text = cleanName;
                        hp.text = currentMoneyTower.TowerSO.Money.ToString();
                        deleyParent.SetActive(false);
                    }
                }
            }

        // 실시간 hp 갱신
        if (panel.activeSelf && currentHealthManager != null && currentTowerSetting != null)
        {
            hp.text = currentHealthManager.nowTowerHealth + " / " + currentTowerSetting.Health;
        }

        else if (panel.activeSelf && currentMoneyTower != null)
        {
            str.text = (currentMoneyTower.TowerSO.Money / currentMoneyTower.TowerSO.WaitTime).ToString() + "/s";
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
        //MoneyManager.Instance.ModifyMoney(TowerGridManager.Instance.TowerCostReturn());
        if (currentSelectedTower != null && TowerGridManager.Instance != null)
        {
            Tower tower = currentSelectedTower.GetComponent<Tower>();
            if (tower != null)
            {
                MoneyManager.Instance.ModifyMoney(TowerGridManager.Instance.TowerCostReturn());
                Vector2Int gridPos = new Vector2Int(tower.gridX, tower.gridY);
                TowerGridManager.Instance.SellTowerAtPosition(gridPos);
                CloseStat();
                Debug.Log($"포탑 판매 완료: {currentSelectedTower.name} at ({gridPos.x}, {gridPos.y})");
            }
        }
    }
}