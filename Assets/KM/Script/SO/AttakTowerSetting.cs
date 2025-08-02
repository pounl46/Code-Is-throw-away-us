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
        [HideInInspector] public bool IsCompleted = false;
        public float BulletSpeed;
        public float AttacingSpeed;
        public bool Slow;
        public SynergyGroup Clone()
        {
            return new SynergyGroup
            {
                SynergyName = this.SynergyName,
                selectedSynergies = new List<Synergy>(this.selectedSynergies),
                ChangeHealth = this.ChangeHealth,
                ChangeDamage = this.ChangeDamage,
                BulletSpeed = this.BulletSpeed,
                AttacingSpeed = this.AttacingSpeed,
                Slow = this.Slow,
                IsCompleted = false // 항상 false로 초기화
            };
        }
    }
    [CreateAssetMenu(fileName = "AttakTowerSetting", menuName = "SO/AttakTowerSetting")]

    public class AttakTowerSetting : ScriptableObject
    {
        public Synergy synergy;
        public string Name;
        public float AttackDamage;
        public float AttackDelay;
        public float AttackDistance;
        public Sprite TowerSprite;
        public float TowerHealth;
        public AudioClip Audio;
        public int Cost;
        public List<SynergyGroup> Synergies;
    }
}