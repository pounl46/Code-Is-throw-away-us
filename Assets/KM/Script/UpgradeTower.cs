using Script.SO;
using UnityEngine;

public class UpgradeTower : MonoBehaviour
{
    private TowerSetting setting;
    private TowerHealthManager healthManager;
    private ThrowScript Throw;
    private AttakTowerSetting settingSO;
    private int NowLevel = 0;
    private void Start()
    {
        setting = GetComponent<TowerSetting>();
        settingSO = setting._attakTowerSetting;
        healthManager = GetComponent<TowerHealthManager>();
        Throw = GetComponent<ThrowScript>();
    }
    public void OnUpgrde()
    {
        if (NowLevel >= settingSO.LevelUpSetting.Count) return;
        setting.Health += settingSO.LevelUpSetting[NowLevel].setMaxHealth;
        setting.attackDamage += settingSO.LevelUpSetting[NowLevel].setAttackDamage;
        setting.attackDelay = settingSO.LevelUpSetting[NowLevel].setAttackDelay;
        healthManager.nowTowerHealth = (int)setting.Health;

        NowLevel++;
    }
}
