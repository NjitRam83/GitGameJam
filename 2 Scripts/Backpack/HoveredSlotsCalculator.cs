using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BackpackSurvivors.Backpack
{
    public class HoveredSlotsCalculator : MonoBehaviour
    {
        [SerializeField] int _gridWidth = 10;
        //[SerializeField] int _gridHeight = 6;

        private List<BackpackCellQuadrant> _backpackCellQuadrants = new List<BackpackCellQuadrant>();

        public void RegisterQuadrant(BackpackCellQuadrant quadrant)
        { 
            _backpackCellQuadrants.Add(quadrant);
        }

        public int GetHoveredSlotId()
        {
            var hoveredQuadrant = GetHoveredQuadrant();
            if (hoveredQuadrant == null) return -1;

            return hoveredQuadrant.BackpackSlotId;
        }

        public List<int> GetPotentialSlotIdsForItem(MovableBackpackElement movableBackpackElement)
        {
            var result = new List<int>();
            var hoveredSlotId = GetHoveredSlotId();
            if(hoveredSlotId == -1) return result;

            result.Add(hoveredSlotId);

            var isRotated = movableBackpackElement.GetIsRotated();
            var width = isRotated ? movableBackpackElement.BackpackItemSizeContainerSO.ItemHeight : movableBackpackElement.BackpackItemSizeContainerSO.ItemWidth;
            var height = isRotated ? movableBackpackElement.BackpackItemSizeContainerSO.ItemWidth : movableBackpackElement.BackpackItemSizeContainerSO.ItemHeight;

            var additionalSlotsHorizontally = GetAdditionalSlotsHorizontally(hoveredSlotId, width);
            var additionalSlotsVertically = GetAdditionalSlotsVertically(hoveredSlotId, height);
            result.AddRange(additionalSlotsHorizontally);
            result.AddRange(additionalSlotsVertically);

            AddMissingSlotsIdsToFormRectangle(result);
            var x = ApplyItemSizeArrayToFoundSquare(result, movableBackpackElement, width, height);

            return x;
        }

        private List<int> ApplyItemSizeArrayToFoundSquare(List<int> rectangleSlotIds, MovableBackpackElement movableBackpackElement, int width, int height)
        {
            if (rectangleSlotIds.Count != height * width) return new List<int>();

            var result = new List<int>();
            rectangleSlotIds.Sort();

            var rectangleSlotIdsIndexCurrentlyChecking = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var index = i * 10 + j;
                    if(movableBackpackElement.IsYes(index))
                    {
                        result.Add(rectangleSlotIds[rectangleSlotIdsIndexCurrentlyChecking]);
                    }

                    rectangleSlotIdsIndexCurrentlyChecking++;
                }
            }

            return result;
        }

        private void AddMissingSlotsIdsToFormRectangle(List<int> currentSlotIds)
        {
            var minTenPart = currentSlotIds.Min(x => x / 10);
            var maxTenPart = currentSlotIds.Max(x => x / 10);
            var minSinglesPart = currentSlotIds.Min(x => x % 10);
            var maxSinglesPart = currentSlotIds.Max(x => x % 10);

            for (int i = minTenPart; i <= maxTenPart; i++)
            {
                for (int j = minSinglesPart; j <= maxSinglesPart; j++)
                {
                    var slotIdInSquare = (10 * i) + j;
                    if (currentSlotIds.Contains(slotIdInSquare)) continue;

                    //Debug.Log($"SlotID {slotIdInSquare} is missing, adding now ");
                    currentSlotIds.Add(slotIdInSquare);
                }
            }
        }

        private List<int> GetAdditionalSlotsHorizontally(int hoveredSlotId, int width)
        {
            var result = new List<int>();

            if (width % 2 == 0)
            {
                var slotsToAddOnEitherSide = (width - 1) / 2;  // (2-1) / 2 = 0 on either side | (4-1) / 2 = 1 on either side
                AddSlotsHorizontally(hoveredSlotId, result, slotsToAddOnEitherSide);
                var extraSlotSideIndicator = HoveredQuadrantIsOnRightSide() ? 1 : -1;
                var furthestSlotOnExtraSlotSide = hoveredSlotId + (extraSlotSideIndicator * slotsToAddOnEitherSide);
                var extraSlotId = furthestSlotOnExtraSlotSide + extraSlotSideIndicator;
                AddSlotIfOnSameRow(hoveredSlotId, extraSlotId, result);
            }
            else
            {
                var slotsToAddOnEitherSide = (width - 1) / 2;  // (3-1) / 2 = 1 on either side | (5-1) / 2 = 2 on either side
                AddSlotsHorizontally(hoveredSlotId, result, slotsToAddOnEitherSide);
            }

            return result;
        }

        private List<int> GetAdditionalSlotsVertically(int hoveredSlotId, int height)
        {
            var result = new List<int>();

            if (height % 2 == 0)
            {
                var slotsToAddOnEitherSide = (height - 1) / 2;  // (2-1) / 2 = 0 on either side | (4-1) / 2 = 1 on either side
                AddSlotsVertically(hoveredSlotId, result, slotsToAddOnEitherSide);
                var extraSlotSideIndicator = HoveredQuadrantIsOnBottomSide() ? 1 : -1;
                var furthestSlotOnExtraSlotSide = hoveredSlotId + (extraSlotSideIndicator * slotsToAddOnEitherSide * _gridWidth);
                var extraSlotId = furthestSlotOnExtraSlotSide + (extraSlotSideIndicator * _gridWidth);
                AddSlotIfOnSameColumn(hoveredSlotId, extraSlotId, result);
            }
            else
            {
                var slotsToAddOnEitherSide = (height - 1) / 2;  // (3-1) / 2 = 1 on either side | (5-1) / 2 = 2 on either side
                AddSlotsVertically(hoveredSlotId, result, slotsToAddOnEitherSide);
            }

            return result;
        }

        private void AddSlotsHorizontally(int hoveredSlotId, List<int> result, int slotsToAddOnEitherSide)
        {
            for (int i = 1; i <= slotsToAddOnEitherSide; i++)
            {
                var leftSlotId = hoveredSlotId - i;
                var rightSlotId = hoveredSlotId + i;
                AddSlotIfOnSameRow(hoveredSlotId, leftSlotId, result);
                AddSlotIfOnSameRow(hoveredSlotId, rightSlotId, result);
            }
        }

        private void AddSlotIfOnSameRow(int hoveredSlotId, int slotIdToAdd, List<int> slotIdList)
        {
            var rowNumberHoveredSlot = hoveredSlotId / 10;
            var rowNumberSlotToAdd = slotIdToAdd / 10;
            if (rowNumberHoveredSlot == rowNumberSlotToAdd)
            { 
                slotIdList.Add(slotIdToAdd);
            }
        }

        private void AddSlotIfOnSameColumn(int hoveredSlotId, int slotIdToAdd, List<int> slotIdList)
        {
            var columnNumberHoveredSlot = hoveredSlotId % 10;
            var columnNumberSlotToAdd = slotIdToAdd % 10;
            if (columnNumberHoveredSlot == columnNumberSlotToAdd)
            {
                slotIdList.Add(slotIdToAdd);
            }
        }

        private void AddSlotsVertically(int hoveredSlotId, List<int> result, int slotsToAddOnEitherSide)
        {
            for (int i = 1; i <= slotsToAddOnEitherSide; i++)
            {
                var leftSlotId = hoveredSlotId - (i * _gridWidth);
                var rightSlotId = hoveredSlotId + (i * _gridWidth);

                AddSlotIfOnSameColumn(hoveredSlotId, leftSlotId, result);
                AddSlotIfOnSameColumn(hoveredSlotId, rightSlotId, result);
            }
        }

        private bool HoveredQuadrantIsOnRightSide()
        {
            var hoveredQuadrant = GetHoveredQuadrant();
            var isRightSide = hoveredQuadrant.QuadrantIdentifier % 2 == 0;
            return isRightSide;
        }

        private bool HoveredQuadrantIsOnBottomSide()
        {
            var hoveredQuadrant = GetHoveredQuadrant();
            var isBottomSide = hoveredQuadrant.QuadrantIdentifier > 2;
            return isBottomSide;
        }

        private BackpackCellQuadrant GetHoveredQuadrant()
        {
            var hoveredQuadrant = _backpackCellQuadrants.FirstOrDefault(q => q.IsCurrentlyHovered);
            return hoveredQuadrant;
        }
    }
}
