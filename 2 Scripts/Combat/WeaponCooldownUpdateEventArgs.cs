using System;

namespace BackpackSurvivors.Combat
{
    public class WeaponCooldownUpdateEventArgs : EventArgs
    {
        public Weapon Weapon { get; set; }

        public WeaponCooldownUpdateEventArgs(Weapon weapon)
        {
            Weapon = weapon;
        }
    }
}
