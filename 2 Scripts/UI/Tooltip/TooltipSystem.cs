using BackpackSurvivors.Items.ScriptableObjectSSS;
using UnityEngine;
namespace BackpackSurvivors.UI.Tooltip
{
    public class TooltipSystem : MonoBehaviour
    {
        private static TooltipSystem _instance;
        private static TooltipTrigger _currentTooltipTrigger;

        [SerializeField] Tooltip _tooltip;
        [SerializeField] ItemTooltip _itemTooltip;

        private void Awake()
        {
            _instance = this;
        }

        public static void Show(string content, TooltipTrigger currentTooltipTrigger, string header = "")
        {
            HideItem();
            _currentTooltipTrigger = currentTooltipTrigger;
            _instance._tooltip.SetText(content, header);
            _instance._tooltip.gameObject.SetActive(true);
        }
        public static void Hide()
        {
            if (_instance != null)
            {
                _instance._tooltip.gameObject.SetActive(false);
            }
        }

        public static void HideIfTriggerIsCorrect(TooltipTrigger tooltipTriggerToHide)
        {
            if (tooltipTriggerToHide == _currentTooltipTrigger)
            {
                _instance._tooltip.gameObject.SetActive(false);
            }
        }

        public static void ShowItem(BackpackItemSO inventoryItemSO, TooltipTrigger currentTooltipTrigger, bool isVendorInventory)
        {
            Hide();
            _currentTooltipTrigger = currentTooltipTrigger;
            _instance._itemTooltip.SetItem(inventoryItemSO, isVendorInventory);
            _instance._itemTooltip.gameObject.SetActive(true);
        }
        public static void HideItem()
        {
            if (_instance != null)
            {
                _instance._itemTooltip.gameObject.SetActive(false);
            }
        }

        public static void HideItemIfTriggerIsCorrect(TooltipTrigger tooltipTriggerToHide)
        {
            if (tooltipTriggerToHide == _currentTooltipTrigger)
            {
                _instance._itemTooltip.gameObject.SetActive(false);
            }
        }
    }
}