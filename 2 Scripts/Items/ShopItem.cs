using BackpackSurvivors.Backpack;
using BackpackSurvivors.Items.ScriptableObjectSSS;
using BackpackSurvivors.MainGame;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] public BackpackItemSO _backpackItem;
    [SerializeField] BackpackItem _backpackItemPrefab;
    BackPackController _backpackGridManager;
    [SerializeField] GameObject _itemContainer;
    [SerializeField] Image _background;
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] Image _priceCoin;
    private bool _itemPurchased = false;
    private BackpackItem _createdItem;

    public bool IsLocked;
    [SerializeField] GameObject _lockedContainer;
    [SerializeField] Image _lockButton;
    [SerializeField] Sprite _lockedSprite;
    [SerializeField] Sprite _unlockedSprite;
    [SerializeField] AudioSource _lockItemAudioSource;
    [SerializeField] AudioSource _unlockItemAudioSource;
    [SerializeField] Animator _shopItemAnimator;

    [SerializeField] GameObject[] _itemsToHideOnDestroy;
    [SerializeField] GameObject ItemContainer;

    [SerializeField] Sprite BackgroundCommon;
    [SerializeField] Sprite BackgroundUncommon;
    [SerializeField] Sprite BackgroundRare;
    [SerializeField] Sprite BackgroundEpic;
    [SerializeField] Sprite BackgroundLegendary;
    [SerializeField] Sprite BackgroundMythic;





    public void ToggleLock(bool audio = true)
    {
        IsLocked = !IsLocked;
        _lockedContainer.SetActive(IsLocked);
        if (IsLocked)
        {
            if (audio)
            {
                //_lockItemAudioSource.Play(0);
                AudioController.instance.PlayAudioSourceAsSfxClip(_lockItemAudioSource);
            }
            _lockButton.sprite = _lockedSprite;
        }
        else
        {
            if (audio)
            {
                //_unlockItemAudioSource.Play(0);
                AudioController.instance.PlayAudioSourceAsSfxClip(_unlockItemAudioSource);
            }
            _lockButton.sprite = _unlockedSprite;
        }
    }

    public void ToggleLockVisibility(bool showLock)
    {
        if (IsLocked)
        {
            ToggleLock(false);
        }        
        _lockButton.gameObject.SetActive(showLock);
    }

    public void SetLock(bool lockState)
    {
        if (IsLocked != lockState)
        {
            ToggleLock(false);
        }
    }

    public int GetItemId()
    {
        if (_backpackItem == null) { return -1; };
        return _backpackItem.Id;
    }

    public void Init(
        BackpackItemSO backpackItem,
        BackPackController backpackGridManager,
        Canvas canvas)
    {

        _backpackGridManager = backpackGridManager;
        _backpackItem = backpackItem;
        _priceText.SetText(_backpackItem.BuyingPrice.ToString());

        _createdItem = _backpackGridManager.CreateBackpackItem(backpackItem, ItemContainer.transform, this);


        switch (backpackItem.ItemQuality)
        {
            case BackpackSurvivors.Shared.Enums.ItemQuality.Common:
                _background.sprite = BackgroundCommon;
                break;
            case BackpackSurvivors.Shared.Enums.ItemQuality.Uncommon:
                _background.sprite = BackgroundUncommon;
                break;
            case BackpackSurvivors.Shared.Enums.ItemQuality.Rare:
                _background.sprite = BackgroundRare;
                break;
            case BackpackSurvivors.Shared.Enums.ItemQuality.Epic:
                _background.sprite = BackgroundEpic;
                break;
            case BackpackSurvivors.Shared.Enums.ItemQuality.Legendary:
                _background.sprite = BackgroundLegendary;
                break;
            case BackpackSurvivors.Shared.Enums.ItemQuality.Mythic:
                _background.sprite = BackgroundMythic;
                break;
            default:
                break;
        }

    }

    public bool TryToBuyItem()
    {
        if (_itemPurchased) return false;
        return _backpackGridManager.TryToBuyItem(_backpackItem);
    }

    internal void ItemBought()
    {
        _itemPurchased = true;
        SetLock(false);
        _priceText.fontSize = 16f;
        _priceText.SetText("Bought");
        ToggleLockVisibility(false);
        _priceCoin.gameObject.SetActive(false);
        ((RectTransform)_createdItem.transform).anchorMin = new Vector2(0, 0);
        ((RectTransform)_createdItem.transform).anchorMax = new Vector2(0, 0);
        ((RectTransform)_createdItem.transform).localPosition = new Vector2(0, 0);
        _backpackGridManager.BuyItem(_createdItem);
    }

    internal void StartDestruction()
    {
        StartCoroutine(MoveAwayAndDestroy());
    }
    IEnumerator MoveAwayAndDestroy()
    {

        if (!IsLocked)
        {
            for (int i = 0; i < _itemsToHideOnDestroy.Length; i++)
            {
                _itemsToHideOnDestroy[i].SetActive(false);
            }
            _shopItemAnimator.SetTrigger("RerollShopItemGoAway");
        }

        yield return new WaitForSeconds(1);
        DoDestruction();
    }

    public void DoDestruction()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}

