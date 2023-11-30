using BackpackSurvivors.MainGame;
using UnityEngine;

namespace BackpackSurvivors.Level
{
    public class ShopVendor : MonoBehaviour
    {
        public bool CanTrigger = true;
        public bool VendorAvailable = false;
        private Player _player;
        // Start is called before the first frame update
        void Start()
        {
            _player = FindFirstObjectByType<Player>();
        }

        private void Update()
        {
            if (_player == null) return;


            float distance = Vector3.Distance(transform.position, _player.transform.position); 
            //Debug.Log("Distance: " + distance);
            if (distance < 1f)
            {
                if (CanTrigger && VendorAvailable)
                {
                    CanTrigger = false;
                    VendorAvailable = false;

                    if (OnShopVendorReached != null)
                    {
                        OnShopVendorReached(this, new ShopVendorReachedEventArgs());
                    }
                }
            }
            else
            {
                VendorAvailable = true;
            }
        }


        public delegate void ShopVendorReachedHandler(object sender, ShopVendorReachedEventArgs e);
        public event ShopVendorReachedHandler OnShopVendorReached;
    }
}