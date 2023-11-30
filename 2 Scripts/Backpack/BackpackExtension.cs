using UnityEngine;
using UnityEngine.EventSystems;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack
{
    public class BackpackExtension : MovableBackpackElement, IDropHandler //, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] public string ExtensionName;

        private bool _justSpawned = true;
        //private CanvasGroup _canvasGroup;

        public void SetCanvasGroupRaycastBlocking(bool blockRaycasts)
        { 
            _canvasGroup.blocksRaycasts = blockRaycasts;
        }

        public override void Init(Canvas canvas, bool locked = true, bool fromShop = true)
        {
            MovableBackpackElementType = MovableBackpackElementType.BackpackExtension;
            //_canvasGroup = GetComponent<CanvasGroup>();
            base.Init(canvas, locked, fromShop);
        }
        public override void Rotate(RotateDirection rotateDirection)
        {
            base.Rotate(rotateDirection);
        }

        public override void DoOnUpdate()
        {
            base.DoOnUpdate();

            if (_justSpawned && FromShop)
            {
                BackPackController.instance.ResolveRewardPosition(this, this.transform.parent);
                _justSpawned = false;
            }
        }

        public override void SetRotateDirection(RotateDirection rotateDirection)
        {
            base.SetRotateDirection(rotateDirection);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (BackPackController.instance.ExtensionHasItemInIt(this) || Locked)
            {
                Locked = true;
            }
            else
            {
                Locked = false;
            }


            if (Locked)
            {
                eventData.pointerDrag = null;
            }
            else
            {
                base.OnBeginDrag(eventData);
                base.BackpackImage.color = new Color(base.BackpackImage.color.r, base.BackpackImage.color.g, base.BackpackImage.color.b, 1f);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            base.OnDrag(eventData);
        }

        public override void SetSuccesfullDrop()
        {
            base.SetSuccesfullDrop();
            BackpackImage.color = new Color(BackpackImage.color.r, BackpackImage.color.g, BackpackImage.color.b, .25f);
        }

        public override void SetFailedDrop()
        {
            base.SetFailedDrop();
            BackpackImage.color = new Color(BackpackImage.color.r, BackpackImage.color.g, BackpackImage.color.b, .25f);
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
                MovableBackpackElement backpackExtension = eventData.pointerDrag.GetComponent<MovableBackpackElement>();
                BackPackController.instance.HandleDropLogic(backpackExtension);
            }
        }

        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //    if (BackPackController.instance.GetCurrentlyDraggingItem() == null) return;
        //}

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //}

        //public void OnPointerDown(PointerEventData eventData)
        //{
        //}
    }
}