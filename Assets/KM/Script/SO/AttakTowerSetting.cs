using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.SO
{
    public enum Synergy
    {
        Archer,
        Poison,
        Cannon,
        Light,
        Magic,
        Ready
    }

    [Serializable]
    public class SynergyGroup
    {
        public string SynergyName;
        public List<Synergy> selectedSynergies;
        [Header("Multiply Changes")]
        public float ChangeHealth;
        public float ChangeDamage;
        [HideInInspector]
        public bool IsCompleted;

        public float BulletSpeed;
        public float AttacingSpeed;
        public bool Slow;
    }
    public class LevelUpTower
    {
        public float setMaxHealth;
        public float setAttackDamage;
        public float setAttackDelay;
    }
    [CreateAssetMenu(fileName = "AttakTowerSetting", menuName = "SO/AttakTowerSetting")]

    public class AttakTowerSetting : ScriptableObject
    {
        public Synergy synergy;
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