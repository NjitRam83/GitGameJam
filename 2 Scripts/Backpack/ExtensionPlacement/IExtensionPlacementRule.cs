using System.Collections.Generic;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack.ExtensionPlacement
{
    public interface IExtensionPlacementRule
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