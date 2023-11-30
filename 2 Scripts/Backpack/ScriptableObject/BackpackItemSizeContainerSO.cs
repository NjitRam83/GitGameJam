using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BackpackItemSizeContainerSO", menuName = "Game/BackpackItemSizeContainer", order = 1)]
    public class BackpackItemSizeContainerSO : ScriptableObject
    {
        [SerializeField] public int Width = 10;
        [SerializeField] public int Size = 60;

        [SerializeField] public Sprite sizeIcon;

        [SerializeField] public BackpackItemSizeSO Rotation0;
        [SerializeField] public BackpackItemSizeSO Rotation90;
        [SerializeField] public BackpackItemSizeSO Rotation180;
        [SerializeField] public BackpackItemSizeSO Rotation270;

        [SerializeField] public int ItemWidth;
        [SerializeField] public int ItemHeight;


        internal bool IsYes(int i, ItemRotation currentRotation)
        {
            switch (currentRotation)
            {
                case ItemRotation.Rotation0:
                    return Rotation0.IsYes(i);
                case ItemRotation.Rotation90:
                    return Rotation90.IsYes(i);
                case ItemRotation.Rotation180:
                    return Rotation180.IsYes(i);
                case ItemRotation.Rotation270:
                    return Rotation270.IsYes(i);
            }
            return false;
        }

        private void Awake()
        {
            Rotation0.Init(Width, Size);
        }
    }
}