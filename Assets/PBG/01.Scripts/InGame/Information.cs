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


}
