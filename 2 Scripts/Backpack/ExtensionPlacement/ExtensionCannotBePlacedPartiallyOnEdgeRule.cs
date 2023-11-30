using System;
using System.Collections.Generic;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack.ExtensionPlacement
{
    public class ExtensionCannotBePlacedPartiallyOnEdgeRule : IExtensionPlacementRule
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

            //Example case:
            //Diff = 19

            //Slot 0 = 0 / 10 = 0 = 0
            //Slot 1 = 1 / 10 = 0.1 = 0
            //Slot 10 = 10 / 10 = 1 = 1
            //Slot 11 = 11 / 10 = 1.1 = 1

            //Slot 19: 19 / 10 = 1.9 = 1
            //Slot 20: 20 / 10 = 2 = 2
            //Slot 29: 29 / 10 = 2.9 = 2
            //Slot 30: 30 / 10 = 3 = 3

            // Diff between Slot 0 and Slot 19 = 1 = CORRECT
            // Diff between Slot 1 and Slot 20 = 2 = INCORRECT --> return false

            highlightState = HighlightState.Available;

            int index = 0;
            double[] baseExtensionSlotValues = new double[extentionTargetCells.Count];
            double[] targetExtensionSlotValues = new double[extentionTargetCells.Count];

            if (_draggingBackpackExtension == null)
            {
                return false;
            }

            // fill with slot indexes
            for (int i = 0; i < _draggingBackpackExtension.BackpackItemSizeContainerSO.Size; i++)
            {
                if (_draggingBackpackExtension.IsYes(i))
                {
                    if (index < baseExtensionSlotValues.Length)
                    {
                        baseExtensionSlotValues[index] = i;
                        index++;
                    }
                    else
                    {
                        highlightState = HighlightState.None;
                        return false;
                    }
                }
            }

            index = 0;
            foreach (var item in extentionTargetCells)
            {
                if (index < targetExtensionSlotValues.Length)
                {
                    targetExtensionSlotValues[index] = item.SlotId;
                    index++;
                }
                else
                {
                    highlightState = HighlightState.None;
                    return false;
                }

            }


            // calculate dividing by width
            for (int i = 0; i < baseExtensionSlotValues.Length; i++)
            {
                baseExtensionSlotValues[i] = Math.Floor(baseExtensionSlotValues[i] / totalWidth);
            }
            for (int i = 0; i < targetExtensionSlotValues.Length; i++)
            {
                targetExtensionSlotValues[i] = Math.Floor(targetExtensionSlotValues[i] / totalWidth);
            }

            // check for any differences in steps
            double baseDiff = baseExtensionSlotValues[0] - targetExtensionSlotValues[0];
            for (int i = 0; i < baseExtensionSlotValues.Length; i++)
            {
                if (baseExtensionSlotValues[i] - targetExtensionSlotValues[i] != baseDiff)
                {
                    highlightState = HighlightState.None;
                    return false;
                }
            }


            //TODO: add fix for top left corner and bottom right corner


            return true;
        }
        public bool ShouldDoHighlight()
        {
            return false;
        }
    }
}