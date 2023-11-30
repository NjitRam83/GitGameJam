using BackpackSurvivors.Backpack.ScriptableObjects;
using BackpackSurvivors.Combat;
using BackpackSurvivors.Combat.ScriptableObjects;
using System;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Items.ScriptableObjectSSS
{

    [CreateAssetMenu(fileName = "BackpackItemSO", menuName = "Game/BackpackItem", order = 1)]
    public class BackpackItemSO : ScriptableObject
    {
        public int Id;
        public string Name;
        public string Description;
        public int BuyingPrice;
        public int SellingPrice;
        public BackpackItemSizeContainerSO BackpackItemSize;
        public ItemQuality ItemQuality;
        public Sprite Icon;
        public Sprite BackpackImage;
        public GameObject ProjectilePrefab;
        public AttackType AttackType;
        public ItemType ItemType;
        public ItemStatsSO ItemStats;
        public int DropChance;
        public AudioClip AudioOnAttack;
        public float AudioVolumeOnAttack = 0.5f;

        public void InintExternalDamageStats(DamageStats damageStatsToInit)
        {
            damageStatsToInit.CritChance = GetStatValue(StatType.CritChancePercentage);
            damageStatsToInit.CritMultiplier = GetStatValue(StatType.CritMultiplier);
            damageStatsToInit.MinDamage = GetStatValue(StatType.MinDamage);
            damageStatsToInit.MaxDamage = GetStatValue(StatType.MaxDamage);
            damageStatsToInit.Piercing = (int)GetStatValue(StatType.Piercing);
        }

        private float GetStatValue(StatType statType, float defaultValue = 0f)
        { 
            if(ItemStats.StatValues.ContainsKey(statType))
            {
                return ItemStats.StatValues[statType];
            }

            return defaultValue;
        }
    }

}