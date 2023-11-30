using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] bool _isEnabled;
        [SerializeField] string _header;
        [SerializeField] string _content;
        [SerializeField] internal bool _instant;
        internal static LTDescr _delay;

        public void SetContent(string headerToSet, string contentToSet)
        {
            _header = headerToSet;
            _content = contentToSet;
        }

        public void ToggleEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        public virtual void ShowTooltip()
        {
            if (_isEnabled)
            {
                if (_instant)
                {
                    TooltipSystem.Show(_content, this, _header);
                }
                else
                {
                    _delay = LeanTween.delayedCall(0.5f, () =>
                    {
                        TooltipSystem.Show(_content, this, _header);
                    });
                }
            }

        }

        public virtual void HideTooltip()
        {
            if (_instant)
            {
                TooltipSystem.Hide();
            }
            else
            {
                if (_delay != null)
                    LeanTween.cancel(_delay.uniqueId);
                TooltipSystem.Hide();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }

        public void OnMouseEnter()
        {
            ShowTooltip();
        }

        public void OnMouseExit()
        {
            HideTooltip();
        }

        public void HideTooltipIfThisWasTarget(TooltipTrigger tooltipTriggerToClean)
        {
            TooltipSystem.HideIfTriggerIsCorrect(tooltipTriggerToClean);
        }
    }
}