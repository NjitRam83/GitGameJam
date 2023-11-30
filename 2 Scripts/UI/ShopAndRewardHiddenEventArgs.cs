using System;

namespace BackpackSurvivors.UI
{
    public class ShopAndRewardHiddenEventArgs : EventArgs
    {
        public bool FromVendor = false;
        public ShopAndRewardHiddenEventArgs(bool fromVendor)
        {
            this.FromVendor = fromVendor;
        }
    }
}
