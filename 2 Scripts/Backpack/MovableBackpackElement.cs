using BackpackSurvivors.Backpack.ScriptableObjects;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Backpack
{
    public abstract class MovableBackpackElement : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public BackpackItemSizeContainerSO BackpackItemSizeContainerSO;
        [SerializeField] internal Canvas _canvas;
        [SerializeField] internal Image BackpackImage;
        [SerializeField] internal Image VisualImage;

        internal RectTransform _rectTransform;
        internal CanvasGroup _canvasGroup;
        private Vector3 _originalPosition;
        private bool _dropWasSucces;
        private ItemRotation _currentRotation;

        public bool GetIsRotated()
        {
            var isRotated = _currentRotation == ItemRotation.Rotation90 || _currentRotation == ItemRotation.Rotation270;
            return isRotated;
        }

        [SerializeField] public bool Locked;
        [SerializeField] public bool FromShop;

        public ItemRotation GetCurrentRotation { get { return _currentRotation; } }

        public MovableBackpackElementType MovableBackpackElementType;

        private bool _initialized;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            DecreaseAlphaHitTresholdOnImages();
        }

        private void Update()
        {
            DoOnUpdate();
        }
        private void DecreaseAlphaHitTresholdOnImages()
        {
            if (VisualImage != null)
            {
                VisualImage.alphaHitTestMinimumThreshold = 0.5f;
            }
        }

        public virtual void DoOnUpdate()
        {
            if (!_initialized)
            {
                Init(_canvas, Locked, FromShop);
                _initialized = true;
            }
        }

        public virtual void Init(Canvas canvas, bool locked = true, bool fromShop = true)
        {
            _canvas = canvas;
            FromShop = fromShop;
            Locked = locked;
        }
        public virtual void Rotate(RotateDirection rotateDirection)
        {
            SetRotateDirection(rotateDirection);
        }
        public bool IsYes(int i)
        {
            return BackpackItemSizeContainerSO.IsYes(i, _currentRotation);
        }

        public virtual void SetRotateDirection(RotateDirection rotateDirection)
        {

            switch (rotateDirection)
            {
                case RotateDirection.Right:

                    transform.Rotate(new Vector3(0, 0, 90));

                    if (_currentRotation == ItemRotation.Rotation0)
                    {
                        _currentRotation = ItemRotation.Rotation270;
                    }
                    else if (_currentRotation == ItemRotation.Rotation90)
                    {
                        _currentRotation = ItemRotation.Rotation0;
                    }
                    else if (_currentRotation == ItemRotation.Rotation180)
                    {
                        _currentRotation = ItemRotation.Rotation90;
                    }
                    else if (_currentRotation == ItemRotation.Rotation270)
                    {
                        _currentRotation = ItemRotation.Rotation180;
                    }
                    break;
                case RotateDirection.Left:
                    transform.Rotate(new Vector3(0, 0, -90));

                    if (_currentRotation == ItemRotation.Rotation0)
                    {
                        _currentRotation = ItemRotation.Rotation90;
                    }
                    else if (_currentRotation == ItemRotation.Rotation90)
                    {
                        _currentRotation = ItemRotation.Rotation180;
                    }
                    else if (_currentRotation == ItemRotation.Rotation180)
                    {
                        _currentRotation = ItemRotation.Rotation270;
                    }
                    else if (_currentRotation == ItemRotation.Rotation270)
                    {
                        _currentRotation = ItemRotation.Rotation0;
                    }
                    break;
            }
        }


        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            BackPackController.instance.UpdateCursor(CursorForm.Grabbing);
            BackPackController.instance.SetCurrentlyDragging(this);
            
            //_originalPosition = transform.position;

            transform.position = Input.mousePosition;

            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;

        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            BackPackController.instance.HandleDragLogic(this);
        }

        public virtual void SetSuccesfullDrop()
        {
            _originalPosition = transform.position;
            _dropWasSucces = true;

        }

        public virtual void SetFailedDrop()
        {
            _dropWasSucces = false;
            StartCoroutine(MoveFromTo(transform, transform.position, GetOriginalPosition(), 1000f));
        }

        IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed)
        {
            float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
            float t = 0;
            while (t <= 1.0f)
            {
                t += step; // Goes from 0 to 1, incrementing by step each time
                objectToMove.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
                yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
            }
            objectToMove.position = b;
        }


        public virtual void RepositionImage(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            if (_dropWasSucces)
            {
                SetSuccesfullDrop();
            }
            else
            {
                SetFailedDrop();
            }
        }

        internal void SetOriginalPosition(Vector3 position)
        {
            _originalPosition = position;
        }

        internal Vector3 GetOriginalPosition()
        {
            return _originalPosition;
        }

    }
}