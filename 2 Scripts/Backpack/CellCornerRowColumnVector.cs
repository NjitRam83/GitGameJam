using UnityEngine;

namespace BackpackSurvivors.Backpack
{
    public class CellCornerRowColumnVector
    {
        public Vector2 TopLeftCellRowColumn;
        public Vector2 BottomRightCellRowColumn;

        public bool IsFilled { get { return TopLeftCellRowColumn != null && BottomRightCellRowColumn != null; } }
    }
}