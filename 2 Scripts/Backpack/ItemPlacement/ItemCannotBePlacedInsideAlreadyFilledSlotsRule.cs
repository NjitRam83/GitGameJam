using System.Collections.Generic;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack.ItemPlacement
{
    public class ItemCannotBePlacedInsideAlreadyFilledSlotsRule : IItemPlacementRule
    {
        public bool IsAllowed(
            List<BackpackGridCell> extentionTargetCells,
            List<BackpackGridCell> backpackCells,
            MovableBackpackElement _draggingBackpackExtension,
            int diff,
            int totalWidth,
            int totalSize,
            out HighlightState highlightState)
        {
            highlightState = HighlightState.Available;

            bool isAllowed = true;
            foreach (var extentionTargetCell in extentionTargetCells)
            {
                if (extentionTargetCell.IsSlotFilled)
                {
                    isAllowed = false;
                    highlightState = HighlightState.Blocked;
                }

            }
            return isAllowed;
        }
        public bool ShouldDoHighlight()
        {
            return false;
        }
    }
}