using System;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack
{
    public class BackpackChangeStateEventArgs : EventArgs
    {
        public BackpackState NewState { get; private set; }
        public BackpackState OldState { get; private set; }

        public bool FromVendor = false;
        public BackpackChangeStateEventArgs(BackpackState oldState, BackpackState newState, bool fromVendor)
        {
            NewState = newState;
            OldState = oldState;
            FromVendor = fromVendor;
        }
    }
}