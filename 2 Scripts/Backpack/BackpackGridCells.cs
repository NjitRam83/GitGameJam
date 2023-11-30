using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Backpack
{
    public class BackpackGridCells : MonoBehaviour
    {

        [SerializeField] public int BackpackItemWidth = 5;
        [SerializeField] public int BackpackItemSize = 5;
        [SerializeField] public bool[] InitialBools;
        public BitArray Bits;
        private void Awake()
        {
            Bits = new BitArray(InitialBools);
        }
    }
}