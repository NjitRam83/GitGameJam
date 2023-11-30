using static BackpackSurvivors.Shared.Enums;
using UnityEngine;
using System.Collections.Generic;
using BackpackSurvivors.Items.ScriptableObjectSSS;
using BackpackSurvivors.Backpack;
using System.Linq;
using System.Globalization;
using System;
using BackpackSurvivors.Combat.Movement;
using BackpackSurvivors.Combat.Projectile_Movements;
using BackpackSurvivors.Combat;
using BackpackSurvivors.Shared;

namespace BackpackSurvivors.Items
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database", order = 1)]
    public class ItemDataBase : ScriptableObject
    {

        CultureInfo ci = new CultureInfo("en-us");

        [Header("Item Rarity")]
        [SerializeField] public Sprite ItemRarity_Common;
        [SerializeField] public Sprite ItemRarity_Uncommon;
        [SerializeField] public Sprite ItemRarity_Rare;
        [SerializeField] public Sprite ItemRarity_Epic;
        [SerializeField] public Sprite ItemRarity_Legendary;
        [SerializeField] public Sprite ItemRarity_Mythic;

        [Header("Item Types")]
        [SerializeField] public Sprite ItemType_Weapon;
        [SerializeField] public Sprite ItemType_Trinket;
        [SerializeField] public Sprite ItemType_Ring;
        [SerializeField] public Sprite ItemType_Shield;
        [SerializeField] public Sprite ItemType_Armor;
        [SerializeField] public Sprite ItemType_Boots;
        [SerializeField] public Sprite ItemType_Gloves;
        [SerializeField] public Sprite ItemType_Amulet;

        [Header("Extensions")]
        [SerializeField] public List<BackpackExtension> AvailableExtensions;

        [Header("Items")]
        [SerializeField] public List<BackpackItemSO> AvailableItems;

        [Header("PassiveStats")]
        [SerializeField] public List<PassiveStatInformation> PassiveStats;

        internal Sprite GetItemTypeSprite(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return ItemType_Weapon;
            }

            return null;
        }

        internal string GetCleanString(ItemQuality itemQuality)
        {
            switch (itemQuality)
            {
                case ItemQuality.Common:
                    return "Common";
                case ItemQuality.Uncommon:
                    return "Uncommon";
                case ItemQuality.Rare:
                    return "Rare";
                case ItemQuality.Epic:
                    return "Epic";
                case ItemQuality.Legendary:
                    return "Legendary";
                case ItemQuality.Mythic:
                    return "Mythic";
            }

            return itemQuality.ToString();
        }

        internal PassiveStatInformation GetPassiveStat(StatType statType)
        {
            return PassiveStats.FirstOrDefault(x => x.StatType == statType);
        }

        internal string GetCleanString(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return "Weapon";
                case ItemType.Trinket:
                    return "Trinket";
                case ItemType.Shield:
                    return "Shield";
                case ItemType.Armor:
                    return "Armor";
                case ItemType.Boots:
                    return "Boots";
                case ItemType.Gloves:
                    return "Gloves";
                case ItemType.Amulet:
                    return "Amulet";
                case ItemType.Ring:
                    return "Ring";
            }

            return itemType.ToString();
        }

        internal List<StatType> AvailableExtensionStatTypes()
        {
            List<StatType> statTypes = new List<StatType>();

            statTypes.Add(StatType.LuckPercentage);
            statTypes.Add(StatType.SpeedPercentage);
            statTypes.Add(StatType.EnemyCount);
            statTypes.Add(StatType.Health);
            statTypes.Add(StatType.DamagePercentage);
            statTypes.Add(StatType.PickupRadiusPercentage);
            statTypes.Add(StatType.DamageReductionPercentage);

            return statTypes;
        }


        internal List<StatType> GetAggresiveStats()
        {
            List<StatType> aggresiveStats = new List<StatType>();
            aggresiveStats.Add(StatType.CritChancePercentage);
            aggresiveStats.Add(StatType.CritMultiplier);
            aggresiveStats.Add(StatType.DamagePercentage);
            aggresiveStats.Add(StatType.FlatDamage);
            aggresiveStats.Add(StatType.MinDamage);
            aggresiveStats.Add(StatType.MaxDamage);
            aggresiveStats.Add(StatType.Piercing); 

            return aggresiveStats;
        }

        internal List<StatType> GetDefensiveStats()
        {
            List<StatType> defensiveStats = new List<StatType>();
            defensiveStats.Add(StatType.Health);
            defensiveStats.Add(StatType.Armor);
            defensiveStats.Add(StatType.DamageReductionPercentage);
            return defensiveStats;
        }

        internal List<StatType> GetUtilityStats(bool isWeapon)
        {
            List<StatType> utilityStats = new List<StatType>();
            utilityStats.Add(StatType.SpeedPercentage);
            utilityStats.Add(StatType.LuckPercentage);
            utilityStats.Add(StatType.PickupRadiusPercentage);
            utilityStats.Add(StatType.EnemyCount);
            if (!isWeapon)
            {
                utilityStats.Add(StatType.WeaponRange);
            }
            return utilityStats;
        }

        internal Dictionary<StatType, float> FilterByKey(List<StatType> filter, SerializableDictionaryBase<StatType, float> source, bool isWeapon)
        {
            Dictionary<StatType, float> map = new Dictionary<StatType, float>();
            foreach (var item in source)
            {
                if (filter.Contains(item.Key))
                {
                    if (!isWeapon || (isWeapon && (item.Key != StatType.MinDamage && item.Key != StatType.MaxDamage)))
                    {
                        map.Add(item.Key, item.Value);
                    }
                }
            }

            return map;
        }

        internal string GetCleanString(StatType statType)
        {
            switch (statType)
            {
                case StatType.CritChancePercentage:
                    return "Crit Chance";
                case StatType.CritMultiplier:
                    return "Crit Multiplier";
                case StatType.Health:
                    return "Health";
                case StatType.DamagePercentage:
                    return "Damage increase";
                case StatType.SpeedPercentage:
                    return "Speed";
                case StatType.LuckPercentage:
                    return "Luck";
                case StatType.CooldownTime:
                    return "Cooldown reduction";
                case StatType.DamageReductionPercentage:
                    return "Less Damage taken";
                case StatType.Armor:
                    return "Armor";
                case StatType.FlatDamage:
                    return "Base damage";
                case StatType.MinDamage:
                    return "Minimum damage";
                case StatType.MaxDamage:
                    return "Maximum damage";
                case StatType.PickupRadiusPercentage:
                    return "Pickup Radius";
                case StatType.EnemyCount:
                    return "More Enemies per Wave";
                case StatType.WeaponRange:
                    return "Weapon Range";
                case StatType.Piercing:
                    return "Piercing";
                default:
                    return string.Empty;
            }
        }
        internal string GetCleanValue(float value, StatType statType)
        {
            var result = string.Empty;

            switch (statType)
            {
                case StatType.CritChancePercentage:
                    result = GetValueAsPercentageString(value); break;
                case StatType.CritMultiplier:
                    result = GetValueAsPercentageString(value + 1); break; //Crit multiplier is added as damage, so a value of 1 means 100% more damage for a multiplier of 200%
                case StatType.Health:
                    result = GetValueAsIntegerString(value * 100); break;
                case StatType.DamagePercentage:
                    result = GetValueAsPercentageString(value); break;
                case StatType.SpeedPercentage:
                    result =  GetValueAsPercentageString(value); break;
                case StatType.LuckPercentage:
                    result = GetValueAsIntegerString(value * 1000); break;
                case StatType.CooldownTime:
                    result =  GetValueAsPercentageString(value); break;
                case StatType.DamageReductionPercentage:
                    result =  GetValueAsPercentageString(value); break;
                case StatType.Armor:
                    result =  GetValueAsIntegerString(value); break;
                case StatType.FlatDamage:
                    result =  GetValueAsIntegerString(value); break;
                case StatType.MinDamage:
                    result =  GetValueAsIntegerString(value); break;
                case StatType.MaxDamage:
                    result =  GetValueAsIntegerString(value); break;
                case StatType.PickupRadiusPercentage:
                    result =  GetValueAsPercentageString(value); break;
                case StatType.EnemyCount:
                    result =  GetValueAsIntegerString(value); break;
                case StatType.WeaponRange:
                    result =  GetValueAsPercentageString(value); break;
                case StatType.Piercing:
                    result =  GetValueAsIntegerString(value); break;
                default:
                    result =  string.Empty; break;
            }

            result = result.Replace('-', '−'); //#hack: Font we use does not have regular hyphen... 
            return result;
        }

        private string GetValueAsPercentageString(float value)
        {
            var valueAsString = value.ToString("P0", ci);
            if (value > 0) valueAsString = "+" + valueAsString;
            return valueAsString;
        }

        private string GetValueAsIntegerString(float value)
        {
            var valueAsString = Convert.ToInt32(value).ToString();
            if(value > 0) valueAsString = "+" + valueAsString;

            return valueAsString;
        }

        internal string GetCleanString(StatType statType, float value, StatTarget statTarget)
        {
            string targetSingle = string.Empty;
            string targetMultiple = string.Empty;
            switch (statTarget)
            {
                case StatTarget.Player:
                    targetSingle = "player";
                    targetMultiple = "players";
                    break;
                case StatTarget.Enemy:
                    targetSingle = "enemy";
                    targetMultiple = "enemies";
                    break;
            }

            var cleanValue = GetCleanValue(value, statType);
            cleanValue = cleanValue.Replace('-', '−'); //#hack: Font we use does not have regular hyphen... 
            var target = targetSingle;
            var suffix = "speed";
            switch (statType)
            {
                case StatType.SpeedPercentage:
                    target = targetSingle;
                    suffix = "speed";
                    break;
                case StatType.EnemyCount:
                    target = (int)value == 1 ? "enemy" : "enemies";
                    suffix = "per group";
                    break;
                case StatType.Health:
                    target = targetMultiple;
                    suffix = "health";
                    break;
                case StatType.DamagePercentage:
                    target = targetMultiple;
                    suffix = "damage";
                    break;
                case StatType.LuckPercentage:
                    target = targetMultiple;
                    suffix = "luck";
                    break;
                case StatType.PickupRadiusPercentage:
                    target = targetMultiple;
                    suffix = "pickup radius";
                    break;
                case StatType.DamageReductionPercentage:
                    target = targetMultiple;
                    suffix = "damage reduction";
                    break;
            }

            return $"{cleanValue} {target} {suffix}";
        }

        internal string GetAttackTypeString(BackpackItemSO item)
        {
            if (item.ProjectilePrefab == null)
            {
                return String.Empty;
            }
            bool circleAroundPlayer = item.ProjectilePrefab.GetComponent<CirclePlayerProjectileMovement>() != null;
            bool straightToTarget = item.ProjectilePrefab.GetComponent<StraightToTargetProjectileMovement>() != null;

            AttackType attackType = item.AttackType;

            if (circleAroundPlayer)
            {
                return "Circling";
            }
            else
            {
                switch (attackType)
                {
                    case AttackType.AttackClosestEnemy:
                        return "Closest enemy";
                    case AttackType.AttackNorth:
                        return "North of the player";
                    case AttackType.AttackEast:
                        return "East of the player";
                    case AttackType.AttackSouth:
                        return "South of the player";
                    case AttackType.AttackWest:
                        return "West of the player";
                    case AttackType.AttackRandomDirection:
                        return "Random direction each wave";
                    case AttackType.AttackCursor:
                        return "Mouse Cursor";
                    case AttackType.AttackRandomDirectionEachAttack:
                        return "Random directions";
                    default:
                        return "UNKNOWN";
                }
            }
        }

        internal string GetPiercingString(BackpackItemSO item)
        {
            if (item.ItemStats.StatValues.ContainsKey(StatType.Piercing) == false) return string.Empty;

            var piercingStat = item.ItemStats.StatValues[StatType.Piercing];

            if (piercingStat == 0)
            {
                return String.Empty;
            }
            else if (piercingStat > 50)
            {
                return "Infinite";
            }
            else
            {
                return String.Format($"{piercingStat} targets");
            }
        }

        internal string GetColorHexcodeForRarity(ItemQuality rarity)
        { 
            switch(rarity)
            {
                case ItemQuality.Common: return Constants.Colors.HexStrings.RarityCommonColor;
                case ItemQuality.Uncommon: return Constants.Colors.HexStrings.RarityUncommonColor;
                case ItemQuality.Rare: return Constants.Colors.HexStrings.RarityRareColor;
                case ItemQuality.Epic: return Constants.Colors.HexStrings.RarityEpicColor;
                case ItemQuality.Legendary: return Constants.Colors.HexStrings.RarityLegendaryColor;
                default: return "999999";
            }
        }
    }
}