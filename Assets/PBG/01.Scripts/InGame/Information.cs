    using TMPro;
    using UnityEngine;

    public class Information : MonoBehaviour
    {
        [SerializeField] private TMP_Text hp;
        [SerializeField] private TMP_Text str;
        [SerializeField] private TMP_Text towerName;
        [SerializeField] private TMP_Text delay;
        [SerializeField] private GameObject panel;
    // [SerializeField] private TMP_Text delay;

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
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

                if (hit.collider != null)
                {
                    currentSelectedTower = hit.collider.gameObject;
                    panel.SetActive(true);
                    // panel.transform.position = Input.mousePosition;
                    towerName.text = hit.collider.name;
                    TowerSetting _towerSetting = hit.collider.GetComponent<TowerSetting>();
                    str.text = "Str : " + _towerSetting.attackDamage.ToString();
                    hp.text = "Hp : " + _towerSetting.Health.ToString();
                    delay.text = "delay : " + _towerSetting.attackDelay.ToString() + "/s";
                }
            }
        }
        public void SellTower()
        {
            if (currentSelectedTower != null && TowerGridManager.Instance != null)
            {
                Tower tower = currentSelectedTower.GetComponent<Tower>();
                if (tower != null)
                {
                    Vector2Int gridPos = new Vector2Int(tower.gridX, tower.gridY);
                    TowerGridManager.Instance.SellTowerAtPosition(gridPos);
                    CloseStat();
                }
            }
        }

    public void CloseStat()
        {
            panel.SetActive(false);
            currentSelectedTower = null;
    }
    }
