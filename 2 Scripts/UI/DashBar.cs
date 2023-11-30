using BackpackSurvivors.MainGame;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI
{
    public class DashBar : MonoBehaviour
    {
        /// the start fill amount value 
        public float StartValue = 1f;
        /// the end goad fill amount value
        public float EndValue = 0f;
        /// the distance to the start or end value at which the class should start lerping
        public float Tolerance = 0.01f;

        [SerializeField] Image _radialImage;
        protected float _newPercent;


        Player _player;
        public void Start()
        {
            _player = GetPlayer();
            CharacterDash2D dashComponent = _player.GetComponent<CharacterDash2D>();
            dashComponent.OnDashbarNeedsUpdate += DashComponent_OnDashbarNeedsUpdate;
        }

        private void DashComponent_OnDashbarNeedsUpdate(object sender, DashbarNeedsUpdateEventArgs e)
        {
            _newPercent = MMMaths.Remap(e.CurrentFuel, e.MinFuel, e.MaxFuel, StartValue, EndValue);

            if (_newPercent != 1)
            {

            }

            if (_radialImage == null) { return; }
            _radialImage.fillAmount = _newPercent;
            if (_radialImage.fillAmount > 1 - Tolerance)
            {
                _radialImage.fillAmount = 1;
            }
            if (_radialImage.fillAmount < Tolerance)
            {
                _radialImage.fillAmount = 0;
            }


            //float percentage = 0f;
            //if (e.CurrentFuel != e.MaxFuel)
            //{

            //}
            //float percentage = e.CurrentFuel / e.MaxFuel;

            //_radialImage.fillAmount = percentage;
        }

        private Player GetPlayer()
        {
            return FindObjectOfType<Player>();
        }
    }
}
