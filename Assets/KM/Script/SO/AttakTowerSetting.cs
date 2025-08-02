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

        private List<SynergyGroup> _initialSynergies;

        private void OnEnable()
        {
            ResetToInitialState();
        }


        public void ResetToInitialState()
        {
            if (_initialSynergies == null) return;

            Synergies = new List<SynergyGroup>();
            foreach (var group in _initialSynergies)
            {
                var newGroup = new SynergyGroup
                {
                    SynergyName = group.SynergyName,
                    selectedSynergies = new List<Synergy>(group.selectedSynergies),
                    ChangeHealth = group.ChangeHealth,
                    ChangeDamage = group.ChangeDamage,
                    BulletSpeed = group.BulletSpeed,
                    AttacingSpeed = group.AttacingSpeed,
                    Slow = group.Slow,
                    IsCompleted = group.IsCompleted
                };
                Synergies.Add(newGroup);
            }
        }
    }
}