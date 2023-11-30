using BackpackSurvivors.Backpack.ScriptableObjects;
using BackpackSurvivors.Items.ScriptableObjectSSS;
using BackpackSurvivors.UI.Tooltip;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack
{
    public class BackpackItem : MovableBackpackElement, IDropHandler
    {
        [SerializeField] BackpackItemSO _backpackItem;
        private ShopItem _shopItem;

        public void SetBackpackItem(BackpackItemSO backpackItemSO)
        {
            _backpackItem = backpackItemSO;
        }
        public BackpackItemSO GetBackpackItem()
        {
            return _backpackItem;   
        }


        public override void Init(Canvas canvas, bool locked = true, bool fromShop = true)
        {
            MovableBackpackElementType = MovableBackpackElementType.Item;

            base.Init(canvas, locked, fromShop);

            if (_backpackItem != null)
            {
                BackpackImage.sprite = _backpackItem.BackpackImage;

                BackpackItemSizeContainerSO = _backpackItem.BackpackItemSize;
                ((RectTransform)transform).sizeDelta = BackPackController.instance.GetItemSize(this);

                ItemTooltipTrigger itemTooltipTrigger = GetComponent<ItemTooltipTrigger>();
                itemTooltipTrigger.SetItemContent(_backpackItem);
                itemTooltipTrigger.IsVendorInventory = fromShop;

                _backpackItem.ItemStats.Init();
            }
        }

        public void SetShopSource(ShopItem shopItem)
        {
            _shopItem = shopItem;
        }

        public override void Rotate(RotateDirection rotateDirection)
        {
            base.Rotate(rotateDirection);
        }

        public override void SetRotateDirection(RotateDirection rotateDirection)
        {
            base.SetRotateDirection(rotateDirection);
        }


        public override void OnBeginDrag(PointerEventData eventData)
        {
            
            if (FromShop && _shopItem != null)
            {
                // Do item logic to pay for item. Block if not enough cash.
                bool paymentFinished = _shopItem.TryToBuyItem();
                if (!paymentFinished)
                {
                    eventData.pointerDrag = null;
                }
                else
                {
                    base.OnBeginDrag(eventData);
                    FromShop = false;
                    _shopItem.ItemBought();


                    ItemTooltipTrigger itemTooltipTrigger = GetComponent<ItemTooltipTrigger>();
                    itemTooltipTrigger.IsVendorInventory = false;
                }
            }

            if (!FromShop)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                HandleDrag(eventData);
            }
            
        }

        private void HandleDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            base.BackpackImage.color = new Color(base.BackpackImage.color.r, base.BackpackImage.color.g, base.BackpackImage.color.b, 1f);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            base.OnDrag(eventData);
        }

        public override void SetSuccesfullDrop()
        {
            base.SetSuccesfullDrop();
            BackpackImage.color = new Color(BackpackImage.color.r, BackpackImage.color.g, BackpackImage.color.b, 1f);
        }

        public override void SetFailedDrop()
        {
            base.SetFailedDrop();
            BackpackImage.color = new Color(BackpackImage.color.r, BackpackImage.color.g, BackpackImage.color.b, 1f);
        }

        public override void RepositionImage(Vector3 newPosition)
        {
            base.RepositionImage(newPosition);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                MovableBackpackElement backpackItem = eventData.pointerDrag.GetComponent<MovableBackpackElement>();
                BackPackController.instance.HandleDropLogic(backpackItem);
            }
        }


    }
}