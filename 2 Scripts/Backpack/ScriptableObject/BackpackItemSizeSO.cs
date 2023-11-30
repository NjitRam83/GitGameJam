using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Backpack.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BackpackItemSizeSO", menuName = "Game/BackpackItemSize", order = 1)]
    public class BackpackItemSizeSO : ScriptableObject
    {

        [SerializeField] public bool[] InitialBools;
        [SerializeField] public int Width = 5;
        [SerializeField] public int Size = 5;
        [SerializeField] public float PivotX = 0.5f;
        [SerializeField] public float PivotY = 0.5f;

        private BitArray _bits;
        public bool IsYes(int index)
        {
            if (_bits == null)
            {
                _bits = new BitArray(InitialBools);
            }
            var invalidIndex = index < 0 || index >= _bits.Length;
            if (invalidIndex) return false;

            return _bits[index];
        }
        private void Awake()
        {
            _bits = new BitArray(InitialBools);
        }

        internal void Init(int width, int size)
        {
            _bits = new BitArray(InitialBools);
            Width = width;
            Size = size;
        }
    }
}