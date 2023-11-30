using BackpackSurvivors.Level.ScriptableObjects;
using System;
using Unity.VisualScripting;

namespace BackpackSurvivors.UI
{
    public class GoLevelEventArgs : EventArgs
    {
        public MountainLevelPoint Level;

        public GoLevelEventArgs(MountainLevelPoint level)
        {
            Level = level;
        }
    }
}
