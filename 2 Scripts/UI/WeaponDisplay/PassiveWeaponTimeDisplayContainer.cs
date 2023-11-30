using BackpackSurvivors.Combat;
using BackpackSurvivors.MainGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace BackpackSurvivors.UI.WeaponDisplay
{
    public class PassiveWeaponTimeDisplayContainer : MonoBehaviour
    {
        [SerializeField] PassiveWeaponTimeDisplay PassiveWeaponTimeDisplayPrefab;


        Dictionary<Guid, PassiveWeaponTimeDisplay> PassiveWeaponTimeDisplays;

        private void Start()
        {
            CombatController combatController = FindAnyObjectByType<CombatController>();
            PassiveWeaponTimeDisplays = new Dictionary<Guid, PassiveWeaponTimeDisplay>();

            if (combatController == null) return;
            combatController.OnWeaponCooldownUpdate += CombatController_OnWeaponCooldownUpdate;
            combatController.OnWeaponReady += CombatController_OnWeaponReady;
            combatController.OnWeaponRegister += CombatController_OnWeaponRegister;
            combatController.OnWeaponsReset += CombatController_OnWeaponsReset;
        }

        private void CombatController_OnWeaponsReset(object sender, WeaponsResetEventArgs e)
        {
            foreach (var passiveWeaponTimeDisplay in PassiveWeaponTimeDisplays)
            {
                passiveWeaponTimeDisplay.Value.gameObject.SetActive(false);
                Destroy(passiveWeaponTimeDisplay.Value);
            }
            PassiveWeaponTimeDisplays = new Dictionary<Guid, PassiveWeaponTimeDisplay>();
        }

        private void CombatController_OnWeaponRegister(object sender, WeaponRegisterEventArgs e)
        {
            if (!PassiveWeaponTimeDisplays.ContainsKey(e.Weapon.UniqueWeaponKey))
            {
                PassiveWeaponTimeDisplay newDisplay = Instantiate(PassiveWeaponTimeDisplayPrefab, transform);
                newDisplay.Init(e.Weapon);
                PassiveWeaponTimeDisplays.Add(e.Weapon.UniqueWeaponKey, newDisplay);
            }
        }

        private void CombatController_OnWeaponReady(object sender, WeaponReadyEventArgs e)
        {
            if (PassiveWeaponTimeDisplays.ContainsKey(e.Weapon.UniqueWeaponKey))
            {
                PassiveWeaponTimeDisplays[e.Weapon.UniqueWeaponKey].DisplayReadyState(e.Weapon);
            }
        }

        private void CombatController_OnWeaponCooldownUpdate(object sender, WeaponCooldownUpdateEventArgs e)
        {
            if (PassiveWeaponTimeDisplays.ContainsKey(e.Weapon.UniqueWeaponKey))
            {
                PassiveWeaponTimeDisplays[e.Weapon.UniqueWeaponKey].UpdateValues(e.Weapon);
            }
        }
    }
}
