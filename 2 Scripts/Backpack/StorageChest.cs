using BackpackSurvivors.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageChest : MonoBehaviour, IDropHandler
{
    [SerializeField] Sprite ChestOpen;
    [SerializeField] Color ChestColorWhenOpen;
    [SerializeField] Sprite ChestClosed;
    [SerializeField] Color ChestColorWhenClosed;
    [SerializeField] Image ChestImage;

    [SerializeField] GameObject _highlightOverlay;

    public void OpenChest()
    {
        ChestImage.sprite = ChestOpen;
        ChestImage.color = ChestColorWhenOpen;
    }
    public void CloseChest()
    {
        ChestImage.sprite = ChestClosed;
        ChestImage.color = ChestColorWhenClosed;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            MovableBackpackElement movableBackpackElement = eventData.pointerDrag.GetComponent<MovableBackpackElement>();
            BackPackController.instance.HandleDropLogic(movableBackpackElement, true);
        }
    }
    public void ItemEnteredStoragezone()
    {
        if (BackPackController.instance.GetCurrentlyDraggingItem() != null)
        {
            _highlightOverlay.SetActive(true);
        }
    }

    public void ItemExitedStoragezone()
    {
        _highlightOverlay.SetActive(false);
    }
}
