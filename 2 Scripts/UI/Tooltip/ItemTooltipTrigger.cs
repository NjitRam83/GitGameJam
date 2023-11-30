using BackpackSurvivors.Items.ScriptableObjectSSS;
using UnityEngine;
using UnityEngine.EventSystems;
namespace BackpackSurvivors.UI.Tooltip
{
    public class ItemTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] BackpackItemSO _backpackItemSO;
        [SerializeField] public bool IsVendorInventory;
        public void SetItemContent(BackpackItemSO inventoryItemSO)
        {
            _backpackItemSO = inventoryItemSO;
        }

        public override void ShowTooltip()
        {
            if (_instant)
            {
                TooltipSystem.ShowItem(_backpackItemSO, this, IsVendorInventory);
            }
            else
            {
                _delay = LeanTween.delayedCall(0.5f, () =>
                {
                    TooltipSystem.ShowItem(_backpackItemSO, this, IsVendorInventory);
                });
            }
        }

        public override void HideTooltip()
        {
            if (_instant)
            {
                TooltipSystem.HideItem();
            }
            else
            {
                if (_delay != null)
                    LeanTween.cancel(_delay.uniqueId);
                TooltipSystem.HideItem();
            }
        }

    }
}