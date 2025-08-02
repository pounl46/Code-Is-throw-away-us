    using System.Collections.Generic;
using UnityEngine;

public class TowerHealthManager : MonoBehaviour
{
    public int nowTowerHealth;
    private TowerSetting towerSett;
    private SpriteRenderer renderer;
    [SerializeField] private List<GameObject> _damageObjs = new List<GameObject>();
    [SerializeField] private GameObject DestroyParticle;
    private void Start()
    {
        towerSett = GetComponent<TowerSetting>();
        renderer = GetComponent<SpriteRenderer>();
        nowTowerHealth = (int)towerSett.Health;
    }

    #if UNITY_EDITOR
    [ContextMenu("TestDamage")]
    private void DamageTest()
    {
        OnDamamge(10);
    }
#endif
    public void OnDamamge(int damamge)
    {
        nowTowerHealth -= damamge;
        float percent = (nowTowerHealth / towerSett.Health);
        renderer.color = new Color(1, percent, percent);

        if (75 > percent * 100 && 50 < percent * 100)
        {
            _damageObjs[0].SetActive(true);
        }
        else if (50 > percent * 100 && 30 < percent * 100)
        {
            _damageObjs[1].SetActive(true);
        }
        else if (30 > percent * 100)
        {
            _damageObjs[2].SetActive(true);
        }
        if (nowTowerHealth <= 0)
        {
            Instantiate(DestroyParticle).transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
