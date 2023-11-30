using BackpackSurvivors.Backpack;
using UnityEngine;

namespace BackpackSurvivors.DEBUG
{
    public class CellCreator : MonoBehaviour
    {
        [SerializeField] Transform _gridParent;
        [SerializeField] GameObject _cellPrefab;

        private void Start()
        {
            for (int i = 0; i < 60; i++)
            {
                var cell = Instantiate(_cellPrefab, _gridParent);
                var backPackGridCell = cell.GetComponent<DebugBackPackGridCell>();
                backPackGridCell.Init(i);
            }
        }
    }
}
