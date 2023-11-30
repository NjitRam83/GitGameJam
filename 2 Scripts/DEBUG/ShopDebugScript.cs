using BackpackSurvivors.Backpack;
using BackpackSurvivors.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDebugScript : MonoBehaviour
{
    [SerializeField] public ItemDataBase ItemDatabase;
    [SerializeField] public BackPackController BackpackGridManager;
    [SerializeField] public GameObject ButtonContainer;
    [SerializeField] public ShopDebugButton ButtonPrefab;

    private void Start()
    {
        foreach (var item in ItemDatabase.AvailableItems)
        {
            var button = Instantiate(ButtonPrefab, ButtonContainer.transform);
            button.text.SetText(item.Name);
            button.BackpackItemSO = item;
            button.BackpackGridManager = BackpackGridManager;
        }
    }
}
