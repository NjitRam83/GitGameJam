using System.Collections.Generic;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack.ExtensionPlacement
{
    public class ExtensionCannotBePlacedOnActiveSlotRule : IExtensionPlacementRule
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
                if (extentionTargetCell.IsSlotAvailableForPlacement)
                {
                    isAllowed = false;
                    highlightState = HighlightState.Blocked;
                }

            }
            return isAllowed;
        }

        public bool ShouldDoHighlight()
        {
            return true;
        }
    }
}