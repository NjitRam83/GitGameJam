using BackpackSurvivors.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellZone : MonoBehaviour, IDropHandler
{
    [SerializeField] Sprite SellOpen;
    [SerializeField] Color SellColorWhenOpen;
    [SerializeField] Sprite SellClosed;
    [SerializeField] Color SellColorWhenClosed;
    [SerializeField] Image SellImage;

    [SerializeField] GameObject _highlightOverlay;

    public void OpenSell()
    {
        SellImage.sprite = SellOpen;
        SellImage.color = SellColorWhenOpen;
    }
    public void CloseSell()
    {
        SellImage.sprite = SellClosed;
        SellImage.color = SellColorWhenClosed;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            MovableBackpackElement movableBackpackElement = eventData.pointerDrag.GetComponent<MovableBackpackElement>();
            if (movableBackpackElement.MovableBackpackElementType == BackpackSurvivors.Shared.Enums.MovableBackpackElementType.Item)
            {
                BackPackController.instance.SellItem(movableBackpackElement);
            }
            else
            {
                movableBackpackElement.SetFailedDrop();
            }
            
            
            // DO SELL
        }
    }

    public void ItemEnteredSellzone()
    {
        if (BackPackController.instance.GetCurrentlyDraggingItem() != null)
        {
            _highlightOverlay.SetActive(true);
        }
    }

    public void ItemExitedSellzone()
    {
        _highlightOverlay.SetActive(false);
    }
}
