using BackpackSurvivors.Combat;
using BackpackSurvivors.UI.Tooltip;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.WeaponDisplay
{
    public class PassiveWeaponTimeDisplay : MonoBehaviour
    {
        [SerializeField] Image Progressbar;
        [SerializeField] Image WeaponIcon;
        [SerializeField] TextMeshProUGUI NameText;
        [SerializeField] TextMeshProUGUI ProgressTimeText;
        [SerializeField] Image GlowImage;



        public void Init(Weapon weapon)
        {
            NameText.SetText(weapon.BackpackItem.Name);
            WeaponIcon.sprite = weapon.BackpackItem.Icon;
            UpdateValues(weapon);
        }

        public void UpdateValues(Weapon weapon)
        {
            
            if (weapon.CurrentCooldown > 0)
            {
                GlowImage.gameObject.SetActive(false);
                Progressbar.fillAmount = (weapon.Cooldown - weapon.CurrentCooldown) / weapon.Cooldown;
                ProgressTimeText.SetText(String.Format("{0}s", weapon.CurrentCooldown.ToString("0.00")));
            }
            else
            {
                Progressbar.fillAmount = 1f;
                ProgressTimeText.SetText(String.Empty);
            }
            
        }

        internal void DisplayReadyState(Weapon weapon)
        {
            GlowImage.gameObject.SetActive(true);
        }
    }
}
