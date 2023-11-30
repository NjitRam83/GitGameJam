namespace BackpackSurvivors.Backpack
{
    public class CornerCells
    {
        public BackpackGridCell TopLeftCell;
        public BackpackGridCell BottomRightCell;

        public bool IsFilled { get { return TopLeftCell != null && BottomRightCell != null; } }
    }
}