using BackpackSurvivors.Enemies;
using System;

namespace BackpackSurvivors.Combat
{
    public class EnemyDamagedEventArgs : EventArgs
    {
        public Enemy DamagedEnemy { get; private set; }

        public EnemyDamagedEventArgs(Enemy damagedEnemy)
        {
                DamagedEnemy = damagedEnemy;
        }

    }
}
