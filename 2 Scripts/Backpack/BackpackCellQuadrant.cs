using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.Backpack
{
    public class BackpackCellQuadrant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] int _quadrantIdentifier;
        private int _backPackSlotId;
        private bool _isCurrentlyHovered;

        public int QuadrantIdentifier => _quadrantIdentifier;
        public int BackpackSlotId => _backPackSlotId;
        public bool IsCurrentlyHovered => _isCurrentlyHovered;

        public void Init(int backPackSlotId)
        {
            _backPackSlotId = backPackSlotId;
            FindObjectOfType<HoveredSlotsCalculator>().RegisterQuadrant(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isCurrentlyHovered = true;
            //Debug.Log($"Entered slot {_backPackSlotId} quadrant {_quadrantIdentifier}");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isCurrentlyHovered = false;
            //Debug.Log($"Exited slot {_backPackSlotId} quadrant {_quadrantIdentifier}");
        }
    }
}
