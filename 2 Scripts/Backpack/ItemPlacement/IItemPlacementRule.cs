using System.Collections.Generic;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack.ItemPlacement
{
    public interface IItemPlacementRule
    {
        bool IsAllowed(
            List<BackpackGridCell> extentionTargetCells,
            List<BackpackGridCell> backpackCells,
            MovableBackpackElement _draggingBackpackExtension,
            int diff,
            int totalWidth,
            int totalSize,
            out HighlightState highlightState);

        bool ShouldDoHighlight();
    }
}