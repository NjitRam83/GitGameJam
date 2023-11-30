using System;

namespace BackpackSurvivors.Combat
{
    public class WeaponAttackEventArgs : EventArgs
    {

        public WeaponAttackEventArgs(Weapon weapon)
        {
            Weapon = weapon;
        }

        public Weapon Weapon { get; }
    }
}
