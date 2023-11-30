using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack
{
    public class BackpackGridCell : MonoBehaviour, IDropHandler //, IPointerDownHandler ,IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] public bool IsSlotAvailableForPlacement;// is the slot open to place items in?
        [SerializeField] public bool IsSlotFilled; // does the slot have an item in it?
        [SerializeField] public int SlotId;


        [SerializeField] internal float X;
        [SerializeField] internal float Y;
        [SerializeField] internal float XMax;
        [SerializeField] internal float YMax;

       

        public BackpackExtension LinkedBackpackExtension { get; set; }
        public BackpackItem LinkedBackpackItem { get; set; }

        [Header("Slot Sprites")]
        [SerializeField] private Sprite _slotAvailableSprite;
        [SerializeField] private Sprite _slotUnavailableSprite;
        [SerializeField] private Image _backgroundObject;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _blockedColor;
        [SerializeField] private Color _availableColor;
        [SerializeField] private Color _readyColor;

        private BackPackController _backpackGridManager;

        public void Setup(int id, bool isAvailable, bool isFilled, BackPackController backpackGridManager)
        {
            SlotId = id;
            IsSlotAvailableForPlacement = isAvailable;
            IsSlotFilled = isFilled;
            _backpackGridManager = backpackGridManager;
            Regenerate();
            UpdateXY();
            InitQuadrants();
        }

        private void InitQuadrants()
        {
            var quadrants = GetComponentsInChildren<BackpackCellQuadrant>();
            foreach (var quadrant in quadrants)
            {
                quadrant.Init(SlotId);
            }
        }

        void UpdateXY()
        {
            X = transform.position.x / _backpackGridManager._canvas.transform.localScale.x;
            Y = transform.position.y / _backpackGridManager._canvas.transform.localScale.y;

            XMax = X + ((RectTransform)transform).rect.width;
            YMax = Y + ((RectTransform)transform).rect.width;
        }
        private void Start()
        {
            UpdateXY();
        }

        private void Update()
        {
            UpdateXY();
        }

        public void Regenerate()
        {
            if (IsSlotAvailableForPlacement)
            {
                _backgroundObject.sprite = _slotAvailableSprite;
            }
            else
            {
                _backgroundObject.sprite = _slotUnavailableSprite;
            }
        }

        public void UpdatePlacementAvailability(bool canPlace)
        {
            IsSlotAvailableForPlacement = canPlace;
        }


        public void LinkToBackpackExtension(BackpackExtension movableBackpackElement)
        {
            LinkedBackpackExtension = movableBackpackElement;
        }
        public void LinkToBackpackItem(BackpackItem movableBackpackElement)
        {
            LinkedBackpackItem = movableBackpackElement;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                MovableBackpackElement backpackExtension = eventData.pointerDrag.GetComponent<MovableBackpackElement>();
                if (backpackExtension != null)
                {
                    _backpackGridManager.HandleDropLogic(backpackExtension);
                }
            }
        }
        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //    if (_backpackGridManager.GetCurrentlyDraggingItem() == null) return;
        //}

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //}

        //public void OnPointerDown(PointerEventData eventData)
        //{
        //}

        public void SetHighlight(HighlightState highlightState)
        {
            switch (highlightState)
            {
                case HighlightState.Blocked:
                    _backgroundObject.color = _blockedColor;
                    break;
                case HighlightState.Available:
                    _backgroundObject.color = _availableColor;
                    break;
                case HighlightState.None:
                    _backgroundObject.color = _readyColor;
                    break;
                default:
                    break;
            }
        }
        public void RemoveHighlight()
        {
            _backgroundObject.color = _defaultColor;
        }
    }
}
