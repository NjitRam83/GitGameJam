using BackpackSurvivors.Backpack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BadDropArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            MovableBackpackElement movableBackpackElement = eventData.pointerDrag.GetComponent<MovableBackpackElement>();
            BackPackController.instance.HandleDropLogic(movableBackpackElement, false, true);
        }
    }
}
