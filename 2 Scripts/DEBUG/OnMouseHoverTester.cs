using BackpackSurvivors.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BackpackSurvivors.DEBUG
{
    public  class OnMouseHoverTester : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] string _quadrantIdentifier;
        private Image _image;
        private int _backPackSlotId;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _backPackSlotId = GetComponentInParent<BackpackGridCell>().SlotId;
            Debug.Log($"Entered slot {_backPackSlotId} quadrant {_quadrantIdentifier}");
            _image.color = new Color(1, 1, 1, 0.5f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _backPackSlotId = GetComponentInParent<BackpackGridCell>().SlotId;
            Debug.Log($"Exited slot {_backPackSlotId} quadrant {_quadrantIdentifier}");
            _image.color = new Color(1, 1, 1, 0.0f);
        }
    }
}
