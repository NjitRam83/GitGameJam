using BackpackSurvivors.Shared;
using BackpackSurvivors.UI;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Combat
{
    public class DamageVisualizer : MonoBehaviour
    {
        internal void ShowDamagePopup(float damage, bool wasCrit, CharacterType damagedType)
        {
            var uiController = FindObjectOfType<UiController>();
            var flooredDamage = Mathf.FloorToInt(damage);
            var color = GetColor(wasCrit, damagedType);
            uiController.InstantiateNumberPopup(transform.position, flooredDamage.ToString(), color);
        }

        private Color GetColor(bool wasCrit, CharacterType damagedType)
        {
            var color = Color.black;

            switch (damagedType)
            {
                case CharacterType.Player:
                    color = wasCrit ? Constants.Colors.PlayerDamagedByCritColor : Constants.Colors.PlayerDamagedColor;
                    break;
                case CharacterType.Enemy:
                    color = wasCrit ? Constants.Colors.EnemyDamagedByCritColor : Constants.Colors.EnemyDamagedColor;
                    break;
            }

            return color;
        }
    }
}
