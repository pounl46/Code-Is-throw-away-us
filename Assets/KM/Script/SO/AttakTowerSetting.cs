using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.SO
{
    [Serializable]
    public class LevelUpTower
    {
        public float setMaxHealth;
        public float setAttackDamage;
        public float setAttackDelay;
        public int NeedMoney;
    }
    [CreateAssetMenu(fileName = "AttakTowerSetting", menuName = "SO/AttakTowerSetting")]

    public class AttakTowerSetting : ScriptableObject
    {
        public string Name;
        public List<LevelUpTower> LevelUpSetting;
        public float AttackDamage;
        public float AttackDelay;
        public float AttackDistance;
        public Sprite TowerSprite;
        public float TowerHealth;
        public AudioClip Audio;
        public int Cost;
    }
}