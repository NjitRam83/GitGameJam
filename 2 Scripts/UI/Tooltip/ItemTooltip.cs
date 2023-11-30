using BackpackSurvivors.Items;
using BackpackSurvivors.Items.ScriptableObjectSSS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.UI.Tooltip
{
    public class ItemTooltip : Tooltip
    {
        [Header("Visuals")]
        [SerializeField] Image _backgroundImage;
        [SerializeField] Image _itemIcon;

        [Header("Backgrounds")]
        [SerializeField] Sprite _regular;
        [SerializeField] Sprite _uncommon;
        [SerializeField] Sprite _rare;
        [SerializeField] Sprite _epic;
        [SerializeField] Sprite _legendary;
        [SerializeField] Sprite _mythic;

        [Header("Core")]
        [SerializeField] ItemDataBase _itemDataBase;


        public void SetItem(BackpackItemSO item, bool isVendorInventory)
        {
            _itemIcon.sprite = item.BackpackItemSize.sizeIcon;
            _itemIcon.SetNativeSize();

            bool IsWeapon = item.ItemType == ItemType.Weapon;
            string content = string.Empty;

            string attackTypeString = _itemDataBase.GetAttackTypeString(item);
            string piercingString = _itemDataBase.GetPiercingString(item);


            switch (item.ItemQuality)
            {
                case ItemQuality.Common:
                    _backgroundImage.sprite = _regular;
                    content += String.Format("<color=#{1}>{0}</color>", item.ItemQuality.ToString().ToUpper(), "FFFFFF");
                    break;
                case ItemQuality.Uncommon:
                    _backgroundImage.sprite = _uncommon;
                    content += String.Format("<color=#{1}>{0}</color>", item.ItemQuality.ToString().ToUpper(), "2DFF49");
                    break;
                case ItemQuality.Rare:
                    _backgroundImage.sprite = _rare;
                    content += String.Format("<color=#{1}>{0}</color>", item.ItemQuality.ToString().ToUpper(), "1696FF");
                    break;
                case ItemQuality.Epic:
                    _backgroundImage.sprite = _epic;
                    content += String.Format("<color=#{1}>{0}</color>", item.ItemQuality.ToString().ToUpper(), "E81CFF");
                    break;
                case ItemQuality.Legendary:
                    _backgroundImage.sprite = _legendary;
                    content += String.Format("<color=#{1}>{0}</color>", item.ItemQuality.ToString().ToUpper(), "FFDA26");
                    break;
                case ItemQuality.Mythic:
                    _backgroundImage.sprite = _mythic;
                    content += String.Format("<color=#{1}>{0}</color>", item.ItemQuality.ToString().ToUpper(), "FF743D");
                    break;
            }

            content += Environment.NewLine;
            content += Environment.NewLine;
            if (IsWeapon)
            {
                content += String.Format("<color=#FFFFFF>{0} ({1} | {2} damage)</color>",
                    item.ItemType,
                    item.ItemStats.StatValues[StatType.MinDamage],
                    item.ItemStats.StatValues[StatType.MaxDamage]);
                content += Environment.NewLine;
                content += String.Format("<color=#FFFFFF>Cooldown: {0}</color>",
                    item.ItemStats.StatValues[StatType.CooldownTime]);
                content += Environment.NewLine;
                content += String.Format("<color=#FFFFFF>Range: {0}</color>",
                    item.ItemStats.StatValues[StatType.WeaponRange]);
                content += Environment.NewLine;
                content = AddStatsToContent(item, content, _itemDataBase.GetAggresiveStats(), "Aggresive", "#FFFFFF", IsWeapon, "#FFFFFF");
                content += String.Format("<color=#FFFFFF>Target: {0}</color>",
                    attackTypeString);

                if (!String.IsNullOrEmpty(piercingString))
                {
                    content += Environment.NewLine;
                    content += String.Format("<color=#FFFFFF>Piercing: {0}</color>",
                    piercingString);
                }

                content += Environment.NewLine;
                content += Environment.NewLine;
            }
            else
            {
                content += String.Format("<color=#FFFFFF>{0}</color>", item.ItemType);
                content += Environment.NewLine;
            }

            if (IsWeapon == false)
            {
                content = AddStatsToContent(item, content, _itemDataBase.GetAggresiveStats(), "Aggresive", "#A9A9A9", IsWeapon);
            }

            content = AddStatsToContent(item, content, _itemDataBase.GetDefensiveStats(), "Defensive", "#A9A9A9", IsWeapon);
            content = AddStatsToContent(item, content, _itemDataBase.GetUtilityStats(IsWeapon), "Utility", "#A9A9A9", IsWeapon);


            if (!string.IsNullOrEmpty(item.Description))
            {
                content += Environment.NewLine;
                content += String.Format("<i><size='14'><color=#DAC482>'{0}'</color></size></i>",
                    item.Description);
            }
            string priceContent = String.Empty;
            //var coinSpriteInText = "<sup><sprite=\"Coin_Sprite_Asset\" name=\"coin\"></sup>";
            if (isVendorInventory)
            {
                priceContent = string.Format("{0}", item.BuyingPrice);
            }
            else
            {
                priceContent = string.Format("{0}", item.SellingPrice);
            }

            base.SetText(content, item.Name, priceContent);
        }

        private string AddStatsToContent(BackpackItemSO item, string content, List<StatType> filter, string title, string titleColor, bool isWeapon, string statColor = "#5A5ADA")
        {
            var stats = _itemDataBase.FilterByKey(filter, item.ItemStats.StatValues, isWeapon);

            foreach (var stat in stats)
            {
                if (stat.Key == StatType.Piercing && isWeapon) continue; //Piercing is added seperatly

                if (stats.Count > 0)
                {
                    var cleanValue = _itemDataBase.GetCleanValue(stat.Value, stat.Key);
                    if(isWeapon && cleanValue.StartsWith("+")) cleanValue= cleanValue.Substring(1); //Strip off + sign before stats for weapons

                    content += String.Format("<color={2}>{0} {1}</color>",
                       cleanValue,
                       _itemDataBase.GetCleanString(stat.Key),
                       statColor);
                    content += Environment.NewLine;
                }
            }

            return content;
        }
    }
}