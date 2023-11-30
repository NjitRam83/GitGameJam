using BackpackSurvivors.Backpack;
using System;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.DEBUG
{
    public class DebugBackPackGridCell : MonoBehaviour
    {
        [SerializeField] public int SlotId;
        [SerializeField] TextMeshProUGUI _slotIdText;

        internal void Init(int i)
        {
            SlotId = i;
            _slotIdText.text = i.ToString();
        }

        private void Start()
        {
            var quadrants = GetComponentsInChildren<BackpackCellQuadrant>();
            foreach (var quadrant in quadrants)
            {
                quadrant.Init(SlotId);
            }
        }
    }
}
