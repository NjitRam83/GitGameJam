using System;

namespace BackpackSurvivors.Combat
{
    public class WeaponReadyEventArgs : EventArgs
    {
        public Weapon Weapon { get; set; }

        public WeaponReadyEventArgs(Weapon weapon)
        {
            Weapon = weapon;
        }
    }
}
