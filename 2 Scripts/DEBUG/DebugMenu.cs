using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{

    [SerializeField] public GameObject ShopContainer;
    [SerializeField] public GameObject ExtensionContainer;
    [SerializeField] public Button ShopButton;
    [SerializeField] public Button ExtensionButton;

    bool showShop = false;

    // Start is called before the first frame update
    void Start()
    {
        ToggleStates();
    }

    public void ChangeState()
    {
        showShop = !showShop;
        ToggleStates();
    }

    private void ToggleStates()
    {
        ShopContainer.SetActive(showShop);
        ShopButton.gameObject.SetActive(showShop);
        ExtensionContainer.SetActive(!showShop);
        ExtensionButton.gameObject.SetActive(!showShop);
    }
}
