using System;

namespace BackpackSurvivors.Combat
{
    public class WeaponRegisterEventArgs : EventArgs
    {
        public Weapon Weapon { get; set; }

        public WeaponRegisterEventArgs(Weapon weapon)
        {
            Weapon = weapon;
        }
    }
}
