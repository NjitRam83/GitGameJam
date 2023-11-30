using BackpackSurvivors.Backpack.ExtensionPlacement;
using BackpackSurvivors.Backpack.ItemPlacement;
using BackpackSurvivors.Items;
using BackpackSurvivors.Items.ScriptableObjectSSS;
using BackpackSurvivors.MainGame;
using BackpackSurvivors.UI.Tooltip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;
using Random = UnityEngine.Random;

namespace BackpackSurvivors.Backpack
{
    public class BackPackController : MonoBehaviour
    {
        public static BackPackController instance;

        private bool StopAfterSetupSingleton()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return true;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            return false;
        }

        [Header("Base")]
        [SerializeField] StatDisplayController _statDisplayController;
        [SerializeField] GameObject BackpackGridContainer;
        [SerializeField] internal Canvas _canvas;
        [SerializeField] BackpackGridCell BackpackGridCellPrefab;

        [SerializeField] Texture2D _grabCursorTexture;
        [SerializeField] Texture2D _defaultCursorTexture;
        [SerializeField] Texture2D _cannotDropCursorTexture; 
        [SerializeField] public ItemDataBase ItemDatabase;

        [SerializeField] private GameObject _shopContainer;
        [SerializeField] private GameObject _rewardContainer;
        [SerializeField] private GameObject _nextWaveContainer;
        [SerializeField] private GameObject _exitShopContainer;
        [SerializeField] private Transform _storageContainerArea;

        [Header("Shop")]
        [SerializeField] public GameObject ShopParent;
        [SerializeField] private ShopItem ShopItemPrefab;
        [SerializeField] private Canvas BackpackCanvas;
        [SerializeField] private Animator _vendorAnimator;
        [SerializeField] public SellZone SellParent;
        [SerializeField] private TextMeshProUGUI _rerollButtonText;
        ShopItem[] createdItems;

        private bool _initialized;

        [Header("Cells")]
        [SerializeField] private int _backpackFirstLoadStartCell;
        private BackpackGridCells _backpackGridCells;
        private List<BackpackGridCell> _backPackSlots;

        [Header("Movables")]
        [SerializeField] Transform ExtensionContainer;
        [SerializeField] Transform ItemContainer;
        [SerializeField] StorageChest StorageChest;
        [SerializeField] GameObject[] BadDropZones;
        private MovableBackpackElement _draggingmovableBackpackElement;
        private List<BackpackGridCell> originalBackpackGridCellSlots;
        private List<BackpackGridCell> _dropTargetCells;
        private List<IExtensionPlacementRule> _extensionPlacementRules;
        private List<IItemPlacementRule> _itemPlacementRules;
        private int _currentDiff;

        [Header("Reward")]
        [SerializeField] public GameObject RewardParent;
        [SerializeField] private ExtensionReward ExtensionRewardPrefab;
        [SerializeField] BackpackItem BackpackItemPrefab;
        [SerializeField] BackpackExtension _firstLoadBackpackExtensionPrefab;
        [SerializeField] BackpackItemSO _firstLoadBackpackItem;
        List<StatType> _weaponStatsToIgnoreWhenGettingBackpackPassives = new List<StatType> { StatType.CritChancePercentage, StatType.CritMultiplier, StatType.MinDamage, StatType.MaxDamage, StatType.CooldownTime, StatType.WeaponRange, StatType.Piercing };
        List<ExtensionReward> _createdRewards;
        private bool _newlyBoughtItemWasPlaced;
        private BackpackExtension _newlyBoughtItem;

        [Header("Scale")]
        private BackpackState currentState;

        [Header("Audiostates")]
        [SerializeField] public AudioSource ExtensionPickupAudio;
        [SerializeField] public AudioSource ExtensionDropAudio;
        [SerializeField] public AudioSource ExtensionInvalidDropAudio;
        [SerializeField] public AudioSource ExtensionRotateAudio;
        [SerializeField] public AudioSource ItemPickupAudio;
        [SerializeField] public AudioSource ItemDropAudio;
        [SerializeField] public AudioSource ItemInvalidDropAudio;
        [SerializeField] public AudioSource RewardPickedAudio;
        [SerializeField] public AudioSource RewardSpawnedAudio;
        [SerializeField] public AudioSource ShopReroll;
        [SerializeField] public AudioSource ShopItemBought;
        [SerializeField] public AudioSource ShopItemSold;
        [SerializeField] public AudioSource ShopItemNotEnoughCoins;
        [SerializeField] AudioSource _buttonClick;

        private bool _canReroll = true;
        private int _rerollCost = 1;

        public int SlotIdFromCells;

        private bool _backPackExtensionsAreBlockingRayCasts = true;

        private void SetAllBackPackExtensionsBlockRaycast(bool blockRaycasts)
        {
            if (_backPackExtensionsAreBlockingRayCasts == blockRaycasts) return;

            _backPackExtensionsAreBlockingRayCasts = blockRaycasts;

            var allExtensions = FindObjectsByType<BackpackExtension>(sortMode: FindObjectsSortMode.None);
            foreach (var extension in allExtensions)
            {
                Debug.Log($"Blocking extension: {blockRaycasts}");
                extension.SetCanvasGroupRaycastBlocking(blockRaycasts);
            }
        }

        private void Start()
        {
            Init();
            SetupExtensionPlacementRules();
            SetupItemPlacementRules();
            ChangeState(BackpackState.Hidden, false);
            UpdateRerollButton();
        }

        private void Awake()
        {
            var stopAwake = StopAfterSetupSingleton();
            if (stopAwake) return;

            _backpackGridCells = GetComponent<BackpackGridCells>();
            _backPackSlots = new List<BackpackGridCell>();
            _dropTargetCells = new List<BackpackGridCell>();
        }

        private void Update()
        {
            float mouseIndex = Input.GetAxis("Mouse ScrollWheel");
            if (mouseIndex > 0 || Input.GetKeyDown(KeyCode.R))
            {
                Rotate(RotateDirection.Right);
            }
            else if (mouseIndex < 0)
            {
                Rotate(RotateDirection.Left);
            }
        }

        internal void Init()
        {
            if (!_initialized)
            {
                for (int i = 0; i < _backpackGridCells.BackpackItemSize; i++)
                {
                    BackpackGridCell cellInstance = Instantiate(BackpackGridCellPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    cellInstance.transform.SetParent(BackpackGridContainer.transform, false);
                    cellInstance.Setup(i, _backpackGridCells.Bits[i], false, this);
                    _backPackSlots.Add(cellInstance);
                }
            }
            _initialized = true;
        }

        public void ChangeState(BackpackState targetState, bool fromVendor)
        {
            switch (targetState)
            {
                case BackpackState.FirstLoad:
                    gameObject.SetActive(true);
                    BackpackCanvas.gameObject.SetActive(true);
                    _rewardContainer.SetActive(true);
                    _shopContainer.SetActive(false);
                    _nextWaveContainer.SetActive(false);
                    SellParent.gameObject.SetActive(false);
                    DoFirstLoadInit();

                    break;
                case BackpackState.Reward:

                    _statDisplayController.Close();

                    gameObject.SetActive(true);
                    BackpackCanvas.gameObject.SetActive(true);
                    _rewardContainer.SetActive(true);
                    _shopContainer.SetActive(false);
                    _nextWaveContainer.SetActive(false);
                    SellParent.gameObject.SetActive(false);
                    CreateRewards();
                    break;
                case BackpackState.Shop:
                    gameObject.SetActive(true);
                    BackpackCanvas.gameObject.SetActive(true);
                    _rewardContainer.SetActive(false);
                    _shopContainer.SetActive(true);
                    SellParent.gameObject.SetActive(true);
                    if (fromVendor)
                    {
                        _nextWaveContainer.SetActive(false);
                        _exitShopContainer.SetActive(true);
                    }
                    else
                    {
                        _exitShopContainer.SetActive(false);
                        _nextWaveContainer.SetActive(true);
                    }
                    CreateShopItems(6, false);
                    break;
                case BackpackState.NextWave:
                    _statDisplayController.Close();
                    gameObject.SetActive(true);
                    BackpackCanvas.gameObject.SetActive(true);
                    _rewardContainer.SetActive(false);
                    _shopContainer.SetActive(false);
                    _nextWaveContainer.SetActive(true);
                    SellParent.gameObject.SetActive(false);
                    break;
                case BackpackState.Hidden:
                    UpdateCursor(CursorForm.Default);
                    _statDisplayController.Close();
                    TooltipSystem.Hide();
                    TooltipSystem.HideItem();
                    BackpackCanvas.gameObject.SetActive(false);
                    _rewardContainer.SetActive(false);
                    _shopContainer.SetActive(false);
                    _nextWaveContainer.SetActive(false);
                    SellParent.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    break;
            }
            BackpackState prevState = currentState;
            currentState = targetState;

            // Make sure someone is listening to event
            if (OnChangeState == null) return;

            BackpackChangeStateEventArgs args = new BackpackChangeStateEventArgs(prevState, targetState, fromVendor);
            OnChangeState(this, args);
        }

        private void DoFirstLoadInit()
        {
            CreateAndDropExtension(_backpackFirstLoadStartCell);
            CreateAndDropItem(_backpackFirstLoadStartCell);
            ChangeState(BackpackState.Shop, false);
        }

        private void CreateAndDropItem(int backpackFirstLoadStartCell)
        {
            // create first item
            var firstItem = CreateBackpackItem(_firstLoadBackpackItem, ItemContainer, null);
            firstItem.FromShop = false;
            SetCurrentlyDraggingItem(firstItem);
            ((RectTransform)firstItem.transform).anchorMin = new Vector2(0, 0);
            ((RectTransform)firstItem.transform).anchorMax = new Vector2(0, 0);

            // Place
            CornerCells firstLoadCell = GetCornerCells(new List<int>() { _backpackFirstLoadStartCell });
            BackpackGridCell firstLoadLefTopCell = firstLoadCell.TopLeftCell;

            float yOffSet = 0f;
            float mod = Screen.height / 600;
            float halfCellHeight = ((RectTransform)firstLoadLefTopCell.transform).sizeDelta.y / 2;
            yOffSet = halfCellHeight * mod;

            firstItem.transform.position = new Vector3(
                firstLoadLefTopCell.transform.position.x,
                firstLoadLefTopCell.transform.position.y + yOffSet,
                0f);




            //Setup
            int diff = 0;
            var itemCellsWithinBounds = GetCellsWithinBoundsFromCellSlot(firstItem, _backpackFirstLoadStartCell + 1, out diff);
            _draggingmovableBackpackElement = null;
            originalBackpackGridCellSlots.Clear();
            foreach (var itemCellWithinBounds in itemCellsWithinBounds)
            {
                itemCellWithinBounds.LinkToBackpackItem((BackpackItem)firstItem);
                itemCellWithinBounds.IsSlotFilled = true;
                itemCellWithinBounds.Regenerate();
            }
            _dropTargetCells = itemCellsWithinBounds;
            firstItem.SetOriginalPosition(firstItem.transform.position);
            //ItemDropped(firstItem);
        }

        private void CreateAndDropExtension(int backpackFirstLoadStartCell)
        {
            // create first extension
            var firstExtension = CreateExtensionItem(_firstLoadBackpackExtensionPrefab, ExtensionContainer, false, false);
            SetCurrentlyDraggingItem(firstExtension);

            // Place
            CornerCells firstLoadCell = GetCornerCells(new List<int>() { _backpackFirstLoadStartCell - 1 });
            BackpackGridCell firstLoadLefTopCell = firstLoadCell.TopLeftCell;
            ((RectTransform)firstExtension.transform).position = new Vector3(
                firstLoadLefTopCell.XMax,
                firstLoadLefTopCell.Y,
                0f);

            // Setup

            int foo = 0;
            _dropTargetCells.Clear();
            _dropTargetCells.AddRange(GetCellsWithinBoundsFromCellSlot(firstExtension, _backpackFirstLoadStartCell + 1, out foo));
            ExtensionDropped(firstExtension);
        }


        #region Item creation

        public void CreateItem(BackpackItemSO item)
        {
            CreateItemAndReturn(item);
        }

        public ShopItem CreateItemAndReturn(BackpackItemSO item)
        {
            var newItem = Instantiate(ShopItemPrefab, ShopParent.transform);
            newItem.Init(item, this, BackpackCanvas);
            return newItem;
        }



        public void CreateShopItems(int itemAmount, bool animate)
        {
            StartCoroutine(CreateShopItemsRoutine(itemAmount, animate));
        }


        IEnumerator CreateShopItemsRoutine(int itemAmount, bool animate)
        {
            BackpackItemSO[] createdItemsToAddToShop = new BackpackItemSO[itemAmount];
            bool[] createdItemsToLockedLayer = new bool[itemAmount];

            if (createdItems == null)
            {
                createdItems = new ShopItem[itemAmount];
            }


            // clear unlocked items
            if (createdItems != null)
            {
                for (int i = 0; i < createdItems.Length; i++)
                {
                    if (createdItems[i] != null)
                    {
                        // store locked items to regen afterwards again
                        if (createdItems[i].IsLocked)
                        {
                            createdItemsToAddToShop[i] = createdItems[i]._backpackItem;
                            createdItemsToLockedLayer[i] = true;
                        }

                        // remove items
                        if (createdItems[i].isActiveAndEnabled)
                        {
                            if (animate)
                            {
                                createdItems[i].StartDestruction();
                            }
                            else
                            {
                                createdItems[i].DoDestruction();
                            }
                            createdItems[i] = null;
                        }
                    }
                }
            }
            if (animate)
            {
                yield return new WaitForSeconds(1);
            }
            DoCreateShopItemsRoutine(itemAmount, createdItemsToAddToShop, createdItemsToLockedLayer);
        }

        public void DoCreateShopItemsRoutine(int itemAmount, BackpackItemSO[] createdItemsToAddToShop, bool[] createdItemsToLockedLayer)
        {
            // check how many items we can create
            int remainingItemsToCreate = 0;
            for (int i = 0; i < createdItemsToAddToShop.Length; i++)
            {
                if (createdItemsToAddToShop[i] == null)
                {
                    remainingItemsToCreate++;
                }
            }

            //create list with all possible item ids, with weight adding to amount
            List<int> itemIds = new List<int>();
            foreach (var availableItem in ItemDatabase.AvailableItems)
            {
                if (GameController.instance.UnlockedRarities.Contains(availableItem.ItemQuality) == false) continue;

                for (int i = 0; i < availableItem.DropChance; i++)
                {
                    itemIds.Add(availableItem.Id);
                }
            }

            // Luck working: every 1% allows for 1 more random iteration. Each iteration picks the highest rarity item
            float luckValue = StatsController.instance.GetStatTypeValue(StatType.LuckPercentage);
            float luckForCalc = (luckValue * 1000) + 1; 

            int rewards = remainingItemsToCreate;
            for (int i = 0; i < rewards; i++)
            {
                BackpackItemSO bestItemFromLuck = CreateSingleShopItem(itemIds, luckForCalc);

                // if item already in shop set, reroll
                while (createdItemsToAddToShop.Any(x => x != null && x.Id == bestItemFromLuck.Id))
                {
                    bestItemFromLuck = CreateSingleShopItem(itemIds, luckForCalc); ;
                }

                // add to first slot
                int firstFreeItemSlot = 0;
                for (int createdItemIndex = 0; createdItemIndex < createdItems.Length; createdItemIndex++)
                {
                    if (createdItemsToAddToShop[createdItemIndex] == null)
                    {
                        firstFreeItemSlot = createdItemIndex;
                    }
                }
                createdItemsToAddToShop[firstFreeItemSlot] = bestItemFromLuck;
            }

            // finished creating items. Add them to the shop and create GO's
            for (int i = 0; i < createdItemsToAddToShop.Length; i++)
            {
                ShopItem createditem = CreateItemAndReturn(createdItemsToAddToShop[i]);
                createditem.SetLock(createdItemsToLockedLayer[i]);
                createdItems[i] = createditem;
            }

            AudioController.instance.PlaySFXClip(RewardSpawnedAudio.clip, RewardSpawnedAudio.volume);
            //RewardSpawnedAudio.Play(0);
            _canReroll = true;
        }

        private BackpackItemSO CreateSingleShopItem(List<int> itemIds, float luckForCalc)
        {
            var highestQualityForItem = ItemQuality.Common;
            BackpackItemSO bestItemFromLuck = null;

            var luckIndex = 0;
            do
            {
                int randomNumber = Random.Range(0, itemIds.Count);
                int itemId = itemIds[randomNumber];
                var item = ItemDatabase.AvailableItems.First(x => x.Id == itemId);
                if (highestQualityForItem < item.ItemQuality || bestItemFromLuck == null)
                {
                    highestQualityForItem = item.ItemQuality;
                    bestItemFromLuck = item;
                }

                luckIndex++;
            } while (luckIndex < luckForCalc);

            return bestItemFromLuck;
        }

        public void RerollShop()
        {
            if (_canReroll == false) return;
            if (MoneyController.instance.TrySpendCoins(_rerollCost) == false) return;

            _rerollCost += 1;
            UpdateRerollButton();
            _canReroll = false;
            _vendorAnimator.SetTrigger("Activate");
            CreateShopItems(6, true);
            //ShopReroll.Play(0);
            AudioController.instance.PlaySFXClip(ShopReroll.clip, ShopReroll.volume);
        }

        public void ResetRerollCost()
        {
            _rerollCost = 1;
            UpdateRerollButton();
        }

        private void UpdateRerollButton()
        {
            var coinSpriteInText = "<sup><sprite=\"Coin_Sprite_Asset\" name=\"coin\"></sup>";
            _rerollButtonText.text = $"REROLL ({_rerollCost} {coinSpriteInText})";
        }

        internal bool TryToBuyItem(BackpackItemSO backpackItem)
        {


            if (MoneyController.instance.CanAfford(backpackItem.BuyingPrice))
            {
                // can pay
                MoneyController.instance.TrySpendCoins(backpackItem.BuyingPrice);
                _vendorAnimator.SetTrigger("Activate");
                //ShopItemBought.Play(0);
                AudioController.instance.PlaySFXClip(ShopItemBought.clip, ShopItemBought.volume);
                return true;
            }
            else
            {
                //ShopItemNotEnoughCoins.Play(0);
                AudioController.instance.PlaySFXClip(ShopItemNotEnoughCoins.clip, ShopItemNotEnoughCoins.volume);
                return false;
            }
        }

        internal void BuyItem(BackpackItem backpackItem)
        {
            float randomX = Random.Range(_storageContainerArea.transform.position.x, ((RectTransform)_storageContainerArea.transform).rect.width);
            float randomY = Random.Range(_storageContainerArea.transform.position.y, ((RectTransform)_storageContainerArea.transform).rect.height);

            backpackItem.SetOriginalPosition(new Vector3(randomX, randomY, 1f));
            backpackItem.transform.SetParent(ItemContainer);
        }

        internal BackpackItem CreateBackpackItem(BackpackItemSO backpackItem, Transform transform, ShopItem shopItem)
        {
            BackpackItem createdItem = Instantiate(BackpackItemPrefab, transform);
            if (shopItem != null)
            {
                createdItem.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            }
            else
            {
                createdItem.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            createdItem.SetBackpackItem(backpackItem);
            createdItem.Init(BackpackCanvas, shopItem != null);
            createdItem.SetShopSource(shopItem);
            return createdItem;
        }

        #endregion

        #region Rewards
        public void CompleteRewardAndShop()
        {
            CloseStats();
            ChangeState(BackpackState.Hidden, false);
        }

        private void CloseStats()
        {
            GetComponentInChildren<StatDisplayController>().Close();
        }

        public void ExitShop()
        {
            //_buttonClick.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(_buttonClick);
            ChangeState(BackpackState.Hidden, true);
        }

        private List<GeneratedPassive> GetGeneratedRewardPassives()
        {
            var allCreatedPassives = new List<GeneratedPassive>();
            _createdRewards.ForEach(r => allCreatedPassives.AddRange(r.NegativePassives));
            _createdRewards.ForEach(r => allCreatedPassives.AddRange(r.PositivePassives));

            return allCreatedPassives;
        }

        public ExtensionReward CreateRewardAndReturn(BackpackExtension extension)
        {
            var newItem = Instantiate(ExtensionRewardPrefab, RewardParent.transform);


            List<GeneratedPassive> generatedNegativePassives = GeneratePassives(1, false);
            var generatedNegativeTargetStatTypeToAvoid = new Tuple<StatTarget, StatType>(generatedNegativePassives[0].StatTarget, generatedNegativePassives[0].PassiveType);
            // ^^ Needs refactoring if we ever get multiple passives per reward, but good enough for now 
            List<GeneratedPassive> generatedPositivePassives = GeneratePassives(1, true, generatedNegativeTargetStatTypeToAvoid);

            newItem.Init(extension,
                generatedNegativePassives,
                generatedPositivePassives,
                this,
                BackpackCanvas);
            return newItem;
        }

        internal BackpackExtension CreateExtensionItem(BackpackExtension backpackExtensionPrefab, Transform transform, bool locked, bool fromShop)
        {
            BackpackExtension firstBackpackExtension = Instantiate(backpackExtensionPrefab, transform);
            firstBackpackExtension.Init(BackpackCanvas, locked, fromShop);

            // set a backup original position in case movement goes incorrect.
            float randomX = Random.Range(_storageContainerArea.transform.position.x, ((RectTransform)_storageContainerArea.transform).rect.width);
            float randomY = Random.Range(_storageContainerArea.transform.position.y, ((RectTransform)_storageContainerArea.transform).rect.height);

            firstBackpackExtension.SetOriginalPosition(new Vector3(randomX, randomY, 1f));

            return firstBackpackExtension;
        }

        private List<GeneratedPassive> GeneratePassives(int generateAmount, bool isPositiveForPlayer, Tuple<StatTarget, StatType> targetStatTypeToAvoid = null)
        {
            List<GeneratedPassive> result = new List<GeneratedPassive>();
            var statTypeTargetCombinations = new List<Tuple<StatTarget, StatType>>();
            var generatedPassives = GetGeneratedRewardPassives();

            foreach (StatTarget target in Enum.GetValues(typeof(StatTarget)))
            {
                foreach (StatType statType in ItemDatabase.AvailableExtensionStatTypes())
                {
                    if (IsValidStatTypeTargetCombination(target, statType) == false) continue;
                    if (TargetStatTypeCombinationAlreadyGenerated(generatedPassives, target, statType)) continue;
                    if (ShouldAvoidTargetStatType(targetStatTypeToAvoid, target, statType)) continue;

                    float amount = ItemDatabase.GetPassiveStat(statType).NegativeWeight * 100;
                    for (int i = 0; i < amount; i++)
                    {
                        statTypeTargetCombinations.Add(new Tuple<StatTarget, StatType>(target, statType));
                    }
                }
            }

            for (int i = 0; i < generateAmount; i++)
            {
                int randomNumber = Random.Range(0, statTypeTargetCombinations.Count);
                var passiveStatTarget = statTypeTargetCombinations[randomNumber].Item1;
                var passiveStatType = statTypeTargetCombinations[randomNumber].Item2;
                PassiveStatInformation passiveStatInformation = ItemDatabase.GetPassiveStat(passiveStatType);

                GeneratedPassive newPassive = new GeneratedPassive();
                newPassive.PassiveType = passiveStatInformation.StatType;

                var randomStatValue = GetPassiveStatValue(passiveStatInformation, passiveStatTarget, isPositiveForPlayer);
                newPassive.PassiveValue = randomStatValue;
                newPassive.StatTarget = passiveStatTarget;

                result.Add(newPassive);
            }

            return result;
        }

        private static bool ShouldAvoidTargetStatType(Tuple<StatTarget, StatType> targetStatTypeToAvoid, StatTarget target, StatType statType)
        {
            var shouldAvoid = targetStatTypeToAvoid != null && target == targetStatTypeToAvoid.Item1 && statType == targetStatTypeToAvoid.Item2;
            //if (shouldAvoid) { Debug.Log($"Avoiding target: {target} type: {statType}"); }
            return shouldAvoid;
        }

        private static bool TargetStatTypeCombinationAlreadyGenerated(List<GeneratedPassive> generatedPassives, StatTarget target, StatType statType)
        {
            var alreadyGenerated = generatedPassives.Any(p => p.PassiveType == statType && p.StatTarget == target);
            //if (alreadyGenerated) { Debug.Log($"Already generated target: {target} type: {statType}"); }
            return alreadyGenerated;
        }

        private bool IsValidStatTypeTargetCombination(StatTarget target, StatType statType)
        {
            switch (statType)
            {
                case StatType.PickupRadiusPercentage:
                case StatType.LuckPercentage:
                case StatType.EnemyCount:
                    {
                        return target == StatTarget.Player;
                    }
                case StatType.CritChancePercentage:
                case StatType.CritMultiplier:
                case StatType.Health:
                case StatType.DamagePercentage:
                case StatType.SpeedPercentage:
                case StatType.CooldownTime:
                case StatType.DamageReductionPercentage:
                case StatType.Armor:
                case StatType.FlatDamage:
                case StatType.MinDamage:
                case StatType.MaxDamage:
                case StatType.WeaponRange:
                    {
                        return true;
                    }
                default:
                    {
                        throw new NotImplementedException($"Stat {statType} is not implemented in BackPackController.IsValidStatTypeTargetCombination() plz add");
                    }
            }
        }

        private float GetPassiveStatValue(PassiveStatInformation passiveStatInformation, StatTarget statTarget, bool isPositiveForPlayer)
        {
            var randomPositiveValue = Random.Range(passiveStatInformation.PostiveMinRange, passiveStatInformation.PostiveMaxRange);
            var randomNegativeValue = Random.Range(passiveStatInformation.NegativeMinRange, passiveStatInformation.NegativeMaxRange);

            if (passiveStatInformation.StatType == StatType.EnemyCount)
            {
                //Enemycount is added to player but is beneficial when it's lower
                return randomNegativeValue;
            }

            //We assume that higher values of a stat are beneficial to the recipient of the stat
            if (statTarget == StatTarget.Player && isPositiveForPlayer || statTarget == StatTarget.Enemy && isPositiveForPlayer == false)
            {
                return randomPositiveValue;
            }
            else if (statTarget == StatTarget.Player && isPositiveForPlayer == false || statTarget == StatTarget.Enemy && isPositiveForPlayer)
            {
                return randomNegativeValue;
            }
            else
            {
                throw new Exception($"This should never happen, check logic. {Environment.NewLine}Stat: {passiveStatInformation.StatType} | StatTarget: {statTarget} | IsPositive: {isPositiveForPlayer}");
            }
        }

        public void CreateRewards()
        {
            if (_createdRewards != null)
            {
                foreach (var createdReward in _createdRewards)
                {
                    if (createdReward != null && createdReward.isActiveAndEnabled)
                    {
                        createdReward.gameObject.SetActive(false);
                        Destroy(createdReward.gameObject);
                    }
                }
                _createdRewards.Clear();
            }

            _createdRewards = new List<ExtensionReward>();
            int rewards = 3;
            for (int i = 0; i < rewards; i++)
            {
                var extension = GetExtension();

                ExtensionReward createdReward = CreateRewardAndReturn(extension);

                _createdRewards.Add(createdReward);
            }

            //RewardSpawnedAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(RewardSpawnedAudio);
        }

        private BackpackExtension GetExtension()
        {
            var maxExtensionSizeAllowed = GetMaxExtensionSizeAllowed();
            var allowedExtensions = ItemDatabase.AvailableExtensions.Where(e => e.BackpackItemSizeContainerSO.Rotation0.InitialBools.Count(b => b) <= maxExtensionSizeAllowed).ToList();

            int randomNumber = Random.Range(0, allowedExtensions.Count());
            var extension = allowedExtensions[randomNumber];
            return extension;
        }

        private int GetMaxExtensionSizeAllowed()
        {
            var numberOfEmptyCellsForExtensions = GetNumberOfEmptyCellsForExtensions();
            switch (numberOfEmptyCellsForExtensions)
            {
                case < 5: return 1;
                case < 10: return 2;
                case < 20: return 3;
                case < 30: return 4;
                case >= 30: return 6;
            }
        }

        private int GetNumberOfEmptyCellsForExtensions()
        {
            var x = 0;
            foreach (var slot in _backPackSlots)
            {
                if (slot.IsSlotAvailableForPlacement) x++;
            }

            var emptyCells = 60 - x;
            return emptyCells;
        }

        internal void SelectReward(ExtensionReward extensionReward)
        {
            extensionReward.Extension.transform.SetParent(ExtensionContainer);
            extensionReward.Extension.Locked = false;
            extensionReward.Extension.FromShop = false;
            foreach (var createdReward in _createdRewards)
            {
                Destroy(createdReward.gameObject);
            }

            foreach (var positivePassive in extensionReward.PositivePassives)
            {
                StatsController.instance.AddPassive(positivePassive);
            }

            foreach (var negativePassive in extensionReward.NegativePassives)
            {
                StatsController.instance.AddPassive(negativePassive);
            }

            //RewardPickedAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(RewardPickedAudio);

            _newlyBoughtItemWasPlaced = false;
            _newlyBoughtItem = extensionReward.Extension;
        }

        internal void ResolveRewardPosition(BackpackExtension backpackExtension, Transform extensionContainerTransform)
        {
            if (backpackExtension.FromShop)
            {
                Vector3 offset = new Vector3(0, 0, 0);
                backpackExtension.transform.localPosition = offset + extensionContainerTransform.position;
            }
        }

        #endregion

        #region Rotation

        private void Rotate(RotateDirection direction)
        {
            // are we currently dragging something to rotate?
            if (_draggingmovableBackpackElement != null)
            {
                _draggingmovableBackpackElement.Rotate(direction);
                //ExtensionRotateAudio.Play(0);
                AudioController.instance.PlayAudioSourceAsSfxClip(ExtensionRotateAudio);

                HandleDragLogic(_draggingmovableBackpackElement);
            }
        }

        #endregion

        #region Cursor

        public void UpdateCursor(CursorForm cursorForm)
        {
            Texture2D targetCursorTexture = _defaultCursorTexture;

            switch (cursorForm)
            {
                case CursorForm.Default:
                    targetCursorTexture = _defaultCursorTexture;
                    break;
                case CursorForm.Grabbing:
                    targetCursorTexture = _grabCursorTexture;
                    break;
                case CursorForm.CannotDrop:
                    targetCursorTexture = _cannotDropCursorTexture;
                    break;
            }


            Cursor.SetCursor(targetCursorTexture, new Vector2(0,0), CursorMode.Auto);
        }

        #endregion

        #region Drag Drop

        internal void HandleDragLogic(MovableBackpackElement movableBackpackElement)
        {
            movableBackpackElement.transform.position = Input.mousePosition;
            ToggleBadDropZones(true);
            StorageChest.OpenChest();
            UpdateCursor(CursorForm.Grabbing);
            switch (movableBackpackElement.MovableBackpackElementType)
            {
                case MovableBackpackElementType.BackpackExtension:
                    HandleExtensionDragLogic(movableBackpackElement);
                    break;
                case MovableBackpackElementType.Item:
                    SetAllBackPackExtensionsBlockRaycast(false);
                    SellParent.OpenSell();
                    HandleItemDragLogic(movableBackpackElement);
                    break;
            }
        }

        private void ToggleBadDropZones(bool setZonesActive)
        {
            for (int i = 0; i < BadDropZones.Length; i++)
            {
                BadDropZones[i].SetActive(setZonesActive);
            }
        }

        internal void HandleDropLogic(MovableBackpackElement movableBackpackElement, bool droppedIntoChest = false, bool alwaysFail = false)
        {
            Shared.Enums.HighlightState highlightState = Shared.Enums.HighlightState.None;

            SetAllBackPackExtensionsBlockRaycast(true);

            if (!droppedIntoChest)
            {
                switch (movableBackpackElement.MovableBackpackElementType)
                {
                    case MovableBackpackElementType.BackpackExtension:
                        HandleExtensionDropLogic(movableBackpackElement, highlightState, alwaysFail);
                        break;
                    case MovableBackpackElementType.Item:
                        HandleItemDropLogic(movableBackpackElement, highlightState, alwaysFail);
                        break;
                }
            }
            else
            {
                switch (movableBackpackElement.MovableBackpackElementType)
                {
                    case MovableBackpackElementType.BackpackExtension:

                        if (originalBackpackGridCellSlots == null)
                        {
                            originalBackpackGridCellSlots = new List<BackpackGridCell>();
                        }
                        foreach (var originalBackpackGridCellSlot in originalBackpackGridCellSlots)
                        {
                            originalBackpackGridCellSlot.IsSlotAvailableForPlacement = false;
                        }

                        originalBackpackGridCellSlots.Clear();
                        break;
                    case MovableBackpackElementType.Item:
                        ItemDropped(movableBackpackElement);
                        break;
                }
            }

            UpdateCursor(CursorForm.Default);

            SellParent.CloseSell();
            StorageChest.CloseChest();
            ToggleBadDropZones(false);
        }

        public void SetCurrentlyDragging(MovableBackpackElement movableBackpackElement)
        {
            switch (movableBackpackElement.MovableBackpackElementType)
            {
                case MovableBackpackElementType.BackpackExtension:
                    SetCurrentlyDraggingExtension(movableBackpackElement);
                    break;
                case MovableBackpackElementType.Item:
                    SetCurrentlyDraggingItem(movableBackpackElement);
                    break;
            }
        }

        public MovableBackpackElement GetCurrentlyDraggingItem()
        {
            return _draggingmovableBackpackElement;
        }

        #region Item Drag

        private void HandleItemDragLogic(MovableBackpackElement movableBackpackElement)
        {
            Shared.Enums.HighlightState highlightState = Shared.Enums.HighlightState.Available;

            var rules = GetRulesThatCauseFailureToPlaceItem(movableBackpackElement, out highlightState);

            bool DoHighlight = true;
            foreach (var item in rules)
            {
                if (DoHighlight == true)
                {
                    DoHighlight = item.ShouldDoHighlight();
                    if (DoHighlight == false) break;
                }
            }

            HighlightCellsWithinBounds(movableBackpackElement, highlightState, DoHighlight);
        }

        private void SetCurrentlyDraggingItem(MovableBackpackElement movableBackpackElement)
        {
            originalBackpackGridCellSlots = new List<BackpackGridCell>();
            _draggingmovableBackpackElement = movableBackpackElement;

            foreach (var item in _backPackSlots.Where(x => x.LinkedBackpackItem == _draggingmovableBackpackElement))
            {
                originalBackpackGridCellSlots.Add(item);

                item.LinkToBackpackItem(null);
                item.IsSlotFilled = false;
                item.Regenerate();
            }

            //ItemPickupAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(ItemPickupAudio);
        }

        #endregion

        #region Item Drop

        private void HandleItemDropLogic(MovableBackpackElement movableBackpackElement, Shared.Enums.HighlightState highlightState, bool alwaysFail = false)
        {

            if (GetRulesThatCauseFailureToPlaceItem(movableBackpackElement, out highlightState).Any() || alwaysFail)
            {
                ItemDropFailed(movableBackpackElement);
            }
            else
            {
                ItemDropped(movableBackpackElement);
            }
        }

        private void ItemDropFailed(MovableBackpackElement movableBackpackElement)
        {
            movableBackpackElement.SetFailedDrop();

            foreach (var item in originalBackpackGridCellSlots)
            {
                item.LinkToBackpackItem((BackpackItem)movableBackpackElement);
                item.IsSlotFilled = true;
                item.Regenerate();
            }

            originalBackpackGridCellSlots.Clear();

            _draggingmovableBackpackElement = null;



            //ItemInvalidDropAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(ItemInvalidDropAudio);
        }

        internal void ItemDropped(MovableBackpackElement movableBackpackElement)
        {
            movableBackpackElement.SetSuccesfullDrop();

            foreach (var item in originalBackpackGridCellSlots)
            {
                item.UpdatePlacementAvailability(true);
                item.Regenerate();
            }


            RepositionImage(movableBackpackElement);
            _draggingmovableBackpackElement = null;
            originalBackpackGridCellSlots.Clear();
            RemoveAllHighlights();

            foreach (var dropTargetCell in _dropTargetCells)
            {
                dropTargetCell.IsSlotFilled = true;
                dropTargetCell.LinkToBackpackItem((BackpackItem)movableBackpackElement);
                dropTargetCell.Regenerate();
            }

            //ItemDropAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(ItemDropAudio);
        }

        #endregion

        #region Extension Drag

        private void HandleExtensionDragLogic(MovableBackpackElement movableBackpackElement)
        {
            Shared.Enums.HighlightState highlightState = Shared.Enums.HighlightState.Available;

            var rules = GetRulesThatCauseFailureToPlaceExtension(movableBackpackElement, out highlightState);

            bool DoHighlight = true;
            foreach (var item in rules)
            {
                if (DoHighlight == true)
                {
                    DoHighlight = item.ShouldDoHighlight();
                }
            }

            HighlightCellsWithinBounds(movableBackpackElement, highlightState, DoHighlight);
        }

        private void SetCurrentlyDraggingExtension(MovableBackpackElement movableBackpackElement)
        {
            originalBackpackGridCellSlots = new List<BackpackGridCell>();
            _draggingmovableBackpackElement = movableBackpackElement;
            foreach (var gridCell in _backPackSlots.Where(x => x.LinkedBackpackExtension == _draggingmovableBackpackElement))
            {
                originalBackpackGridCellSlots.Add(gridCell);

                gridCell.LinkToBackpackExtension(null);
                gridCell.UpdatePlacementAvailability(false);
                gridCell.Regenerate();
            }
        }

        #endregion

        #region Extension Drop

        private void HandleExtensionDropLogic(MovableBackpackElement movableBackpackElement, Shared.Enums.HighlightState highlightState, bool alwaysFail = false)
        {

            if (GetRulesThatCauseFailureToPlaceExtension(movableBackpackElement, out highlightState).Any() || alwaysFail)
            {
                ExtensionDropFailed(movableBackpackElement);
            }
            else
            {
                ExtensionDropped(movableBackpackElement);
            }
        }

        internal void ExtensionDropped(MovableBackpackElement movableBackpackElement)
        {
            movableBackpackElement.SetSuccesfullDrop();

            RepositionImage(movableBackpackElement);
            _draggingmovableBackpackElement = null;
            originalBackpackGridCellSlots.Clear();
            RemoveAllHighlights();

            // update active and available for placement layers
            foreach (var dropTargetCell in _dropTargetCells)
            {
                dropTargetCell.UpdatePlacementAvailability(true);
                dropTargetCell.LinkToBackpackExtension((BackpackExtension)movableBackpackElement);
                dropTargetCell.Regenerate();
            }

            //ExtensionDropAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(ExtensionDropAudio);

            if (_newlyBoughtItemWasPlaced == false && _newlyBoughtItem == movableBackpackElement)
            {
                ChangeState(BackpackState.Shop, false);
                _newlyBoughtItemWasPlaced = true;
            }
            movableBackpackElement.SetOriginalPosition(movableBackpackElement.transform.position);
        }

        private void ExtensionDropFailed(MovableBackpackElement movableBackpackElement)
        {
            movableBackpackElement.SetFailedDrop();

            foreach (var originalBackpackGridCellSlot in originalBackpackGridCellSlots)
            {
                originalBackpackGridCellSlot.LinkToBackpackExtension((BackpackExtension)movableBackpackElement);
                originalBackpackGridCellSlot.UpdatePlacementAvailability(true);
                originalBackpackGridCellSlot.Regenerate();
            }
            originalBackpackGridCellSlots.Clear();

            RemoveAllHighlights();
            //ExtensionInvalidDropAudio.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(ExtensionInvalidDropAudio);
        }

        #endregion

        #endregion

        #region Item placement rules
        private void SetupItemPlacementRules()
        {

            _itemPlacementRules = new List<IItemPlacementRule>();
            _itemPlacementRules.Add(new ItemCannotBePlacedOutsideOfFillableSlotsRule());
            _itemPlacementRules.Add(new ItemCannotBePlacedInsideAlreadyFilledSlotsRule());
            _itemPlacementRules.Add(new ItemCannotBePlacedPartiallyOnEdgeRule());
        }

        internal List<IItemPlacementRule> GetRulesThatCauseFailureToPlaceItem(
            List<BackpackGridCell> extensionTargetCells,
            out Shared.Enums.HighlightState highlightState)
        {
            highlightState = Shared.Enums.HighlightState.Available;
            List<IItemPlacementRule> failureRules = new List<IItemPlacementRule>();
            foreach (var extensionPlacementRule in _itemPlacementRules)
            {
                if (!extensionPlacementRule.IsAllowed(
                    extensionTargetCells,
                    _backPackSlots,
                    _draggingmovableBackpackElement,
                    _currentDiff,
                    _backpackGridCells.BackpackItemWidth,
                    _backpackGridCells.BackpackItemSize,
                    out highlightState))
                {
                    failureRules.Add(extensionPlacementRule);
                }
            }
            return failureRules;
        }

        internal List<IItemPlacementRule> GetRulesThatCauseFailureToPlaceItem(
            MovableBackpackElement movableBackpackElement,
            out Shared.Enums.HighlightState highlightState)
        {
            return GetRulesThatCauseFailureToPlaceItem(GetCellsWithinBounds(movableBackpackElement), out highlightState);
        }

        #endregion 

        #region Extension placement rules

        private void SetupExtensionPlacementRules()
        {
            _extensionPlacementRules = new List<IExtensionPlacementRule>();
            _extensionPlacementRules.Add(new ExtensionCannotBePlacedPartiallyOnEdgeRule());
            _extensionPlacementRules.Add(new ExtensionCannotBePlacedOnActiveSlotRule());
        }

        internal List<IExtensionPlacementRule> GetRulesThatCauseFailureToPlaceExtension(
            List<BackpackGridCell> extensionTargetCells,
            out Shared.Enums.HighlightState highlightState)
        {
            highlightState = Shared.Enums.HighlightState.Available;
            List<IExtensionPlacementRule> failureRules = new List<IExtensionPlacementRule>();
            foreach (var extensionPlacementRule in _extensionPlacementRules)
            {
                if (!extensionPlacementRule.IsAllowed(
                    extensionTargetCells,
                    _backPackSlots,
                    _draggingmovableBackpackElement,
                    _currentDiff,
                    _backpackGridCells.BackpackItemWidth,
                    _backpackGridCells.BackpackItemSize,
                    out highlightState))
                {
                    failureRules.Add(extensionPlacementRule);
                }
            }
            return failureRules;
        }

        internal List<IExtensionPlacementRule> GetRulesThatCauseFailureToPlaceExtension(MovableBackpackElement movableBackpackElement, out Shared.Enums.HighlightState highlightState)
        {
            return GetRulesThatCauseFailureToPlaceExtension(GetCellsWithinBounds(movableBackpackElement), out highlightState);
        }

        private List<BackpackGridCell> GetCellsWithinBoundsFromCellSlot(MovableBackpackElement movableBackpackElement, int slotId, out int currentDiff)
        {
            List<BackpackGridCell> result = new List<BackpackGridCell>();
            // find the offset between backpack grid and extension size grid
            currentDiff = CalculateDiff(movableBackpackElement, slotId);

            // iterate through all places relative from that point on the actual bag set using the diff
            for (int i = 0; i < _backpackGridCells.BackpackItemSize - currentDiff; i++)
            {
                if (movableBackpackElement.IsYes(i))
                {
                    int backpackCellSlot = i + currentDiff;

                    // grab the slot from list
                    var invalidSlot = backpackCellSlot < 0 || backpackCellSlot >= _backPackSlots.Count();
                    if (invalidSlot) continue;

                    BackpackGridCell foundCellSlot = _backPackSlots[backpackCellSlot];
                    result.Add(foundCellSlot);
                }
            }
            return result;
        }
        internal List<BackpackGridCell> GetCellsWithinBounds(MovableBackpackElement movableBackpackElement)
        {
            _dropTargetCells.Clear();

            if (Input.GetKeyDown(KeyCode.U))
            { 
                Debug.Log("yes");
            }

            // figure out what cell we are top most left at
            int slotId = GetFirstSlotOnMovableDrag(movableBackpackElement);
            var hoverCalculator = GetComponentInChildren<HoveredSlotsCalculator>();
            slotId = hoverCalculator.GetHoveredSlotId();

            if (slotId >= 0)
            {
                // find the offset between backpack grid and extension size grid
                //_dropTargetCells.AddRange(GetCellsWithinBoundsFromCellSlot(movableBackpackElement, slotId, out _currentDiff));
                var dropTargetCellIds = hoverCalculator.GetPotentialSlotIdsForItem(movableBackpackElement);
                var cellsToAdd = _backPackSlots.Where(i => dropTargetCellIds.Contains(i.SlotId)).ToList();
                _dropTargetCells.AddRange(cellsToAdd);
            }

            return _dropTargetCells;
        }

        private int CalculateDiff(MovableBackpackElement movableBackpackElement, int slotId)
        {
            int diff = -1;
            int index = 0;
            while (diff < 0 && index < _backpackGridCells.BackpackItemSize)
            {
                if (movableBackpackElement.IsYes(index))
                {
                    // calculate the difference between extension slot and backpack slot
                    diff = slotId - index; //example: we point at slot 3, extension slot(i) 0 is yes, diff is 3
                }
                index++;
            }

            return diff;
        }

        private int GetFirstSlotOnMovableDrag(MovableBackpackElement movableBackpackElement)
        {
            var elementWidth = ((RectTransform)movableBackpackElement.transform).sizeDelta.x;
            var elementHeight = ((RectTransform)movableBackpackElement.transform).sizeDelta.y;

            var numberOfCellsXOffset = elementWidth / 48;
            var numberOfCellsYOffset = elementHeight / 48;

            numberOfCellsXOffset--;
            numberOfCellsYOffset--;

            numberOfCellsXOffset = Mathf.Clamp(numberOfCellsXOffset, 0, numberOfCellsXOffset);
            numberOfCellsYOffset = Mathf.Clamp(numberOfCellsYOffset, 0, numberOfCellsYOffset);

            var movableIsRotated = movableBackpackElement.GetCurrentRotation == ItemRotation.Rotation90 || movableBackpackElement.GetCurrentRotation == ItemRotation.Rotation270;

            if (movableIsRotated)
            {
                var temp = numberOfCellsXOffset;
                numberOfCellsXOffset = numberOfCellsYOffset;
                numberOfCellsYOffset = temp;
            }

            var rectTransform = (RectTransform)movableBackpackElement.transform;
            var offsetX = rectTransform.offsetMax.x;
            var offsetY = rectTransform.offsetMax.y;

            var topLeftCellX = offsetX - (numberOfCellsXOffset * 48);
            var topLeftCellY = offsetY - (numberOfCellsYOffset * 48);

            if (movableIsRotated) { topLeftCellX = offsetX - ((numberOfCellsXOffset + (numberOfCellsYOffset * 0.5f)) * 48); } //DONT ASK WHY !!!!
            if (movableIsRotated) { topLeftCellY = offsetY + numberOfCellsYOffset * 0.5f * 48; } //DONT ASK WHY !!!!

            Vector3 topLeftCenter = new Vector3(topLeftCellX, topLeftCellY, 0f);

            foreach (var slot in _backPackSlots)
            {
                float cell_x_min = slot.X;
                float cell_y_min = slot.Y;
                float cell_x_max = slot.XMax;
                float cell_y_max = slot.YMax;

                if (cell_x_min <= topLeftCenter.x && cell_x_max > topLeftCenter.x &&
                    cell_y_min <= topLeftCenter.y && cell_y_max > topLeftCenter.y)
                {
                    return slot.SlotId;
                }
            }
            return -1;
        }

        internal Vector2 GetItemSize(MovableBackpackElement movableBackpackElement)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < 60; i++)
            {
                if (movableBackpackElement.IsYes(i))
                {
                    indexes.Add(i);
                }
            }

            CornerCells cornerCells = GetCornerCells(indexes);

            if (!cornerCells.IsFilled) { return new Vector2(0, 0); }

            CellCornerRowColumnVector cellCornerRowColumnVector = GetCornerCellVectors(indexes);

            if (!cellCornerRowColumnVector.IsFilled) { return new Vector2(0, 0); }

            float height = (cellCornerRowColumnVector.BottomRightCellRowColumn.x - cellCornerRowColumnVector.TopLeftCellRowColumn.x + 1) * 48;
            float width = (cellCornerRowColumnVector.BottomRightCellRowColumn.y - cellCornerRowColumnVector.TopLeftCellRowColumn.y + 1) * 48;

            Vector2 result = new Vector2(width, height);

            return result;
        }

        internal void RepositionImage(MovableBackpackElement movableBackpackElement)
        {
            CornerCells cornerCells = GetCornerCells(_dropTargetCells.Select(x => x.SlotId).ToList());

            if (cornerCells == null || !cornerCells.IsFilled) { return; }

            var centerPositionForImage = (
                (cornerCells.TopLeftCell.transform.position) +
                (cornerCells.BottomRightCell.transform.position)) / 2;

            movableBackpackElement.RepositionImage(centerPositionForImage);
        }

        internal void RepositionImageUsingOwnCell(MovableBackpackElement movableBackpackElement)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < 60; i++)
            {
                if (movableBackpackElement.IsYes(i))
                {
                    indexes.Add(i);
                }
            }

            CornerCells cornerCells = GetCornerCells(indexes);

            if (!cornerCells.IsFilled) { return; }

            var centerPositionForImage = (cornerCells.TopLeftCell.transform.position + cornerCells.BottomRightCell.transform.position) / 2;

            movableBackpackElement.RepositionImage(centerPositionForImage);
        }

        internal CornerCells GetCornerCells(List<int> targetCells)
        {

            CellCornerRowColumnVector cellCornerRowColumnVector = GetCornerCellVectors(targetCells);
            if (cellCornerRowColumnVector == null || !cellCornerRowColumnVector.IsFilled) { return null; }

            int topLeftRectangleCellIndex = (int)(cellCornerRowColumnVector.TopLeftCellRowColumn.x * 10 + cellCornerRowColumnVector.TopLeftCellRowColumn.y);
            int bottomRightRectangleCellIndex = (int)(cellCornerRowColumnVector.BottomRightCellRowColumn.x * 10 + cellCornerRowColumnVector.BottomRightCellRowColumn.y);

            BackpackGridCell topLeftCell = _backPackSlots.FirstOrDefault(x => x.SlotId == topLeftRectangleCellIndex);
            BackpackGridCell bottomRightCell = _backPackSlots.FirstOrDefault(x => x.SlotId == bottomRightRectangleCellIndex);

            CornerCells cornerCells = new CornerCells() { TopLeftCell = topLeftCell, BottomRightCell = bottomRightCell };

            return cornerCells;
        }

        internal CellCornerRowColumnVector GetCornerCellVectors(List<int> targetCells)
        {
            List<int> rowIndexes = new List<int>();
            List<int> columnIndexes = new List<int>();

            foreach (var slotId in targetCells)
            {
                int rowIndex = Mathf.FloorToInt(slotId / 10);
                int columnIndex = slotId % 10;

                rowIndexes.Add(rowIndex);
                columnIndexes.Add(columnIndex);
            }

            if (rowIndexes.Count == 0 || columnIndexes.Count == 0) { return null; }

            rowIndexes = rowIndexes.OrderBy(x => x).ToList();
            columnIndexes = columnIndexes.OrderBy(x => x).ToList();
            int topLeftRowIndex = rowIndexes[0];
            int topLeftColumnIndex = columnIndexes[0];


            rowIndexes = rowIndexes.OrderByDescending(x => x).ToList();
            columnIndexes = columnIndexes.OrderByDescending(x => x).ToList();
            int bottomRightRowIndex = rowIndexes[0];
            int bottomRightColumnIndex = columnIndexes[0];

            CellCornerRowColumnVector cellCornerRowColumnVector = new CellCornerRowColumnVector()
            {
                TopLeftCellRowColumn = new Vector2(topLeftRowIndex, topLeftColumnIndex),
                BottomRightCellRowColumn = new Vector2(bottomRightRowIndex, bottomRightColumnIndex),
            };

            return cellCornerRowColumnVector;
        }

        #endregion

        #region Highlights

        private void RemoveAllHighlights()
        {
            foreach (var backPackSlot in _backPackSlots)
            {
                backPackSlot.RemoveHighlight();
            }
        }

        public void HighlightCellsWithinBounds(
            MovableBackpackElement movableBackpackElement,
            Shared.Enums.HighlightState highlightState,
            bool doHighlight)
        {
            RemoveAllHighlights();

            if (doHighlight)
            {
                foreach (var cellWithinBounds in GetCellsWithinBounds(movableBackpackElement))
                {
                    cellWithinBounds.SetHighlight(highlightState);
                }
            }
        }

        internal bool ExtensionHasItemInIt(BackpackExtension backpackExtension)
        {
            foreach (var item in _backPackSlots.Where(x => x.LinkedBackpackExtension == backpackExtension))
            {
                if (item.IsSlotFilled)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Items

        public List<BackpackItem> GetAllWeapons()
        {
            List<BackpackItem> allWeapons = new List<BackpackItem>();

            foreach (var backPackSlot in _backPackSlots)
            {
                if (backPackSlot.IsSlotFilled)
                {
                    BackpackItem item = backPackSlot.LinkedBackpackItem;
                    if (item.GetBackpackItem().ItemType == ItemType.Weapon)
                    {
                        if (!allWeapons.Contains(item))
                        {
                            allWeapons.Add(item);
                        }
                    }
                }
            }

            return allWeapons;
        }

        public Dictionary<StatType, float> CalculatePlayerPassivesFromBackpack()
        {
            Dictionary<StatType, float> statValues = new Dictionary<StatType, float>();

            foreach (StatType statValueType in Enum.GetValues(typeof(StatType))) { statValues.Add(statValueType, 0f); }

            List<BackpackItem> itemsAlreadyCalculated = new List<BackpackItem>();
            foreach (var backPackSlot in _backPackSlots)
            {
                if (backPackSlot.IsSlotFilled == false) continue;

                BackpackItem backpackItem = backPackSlot.LinkedBackpackItem;

                if (itemsAlreadyCalculated.Any(x => x.GetInstanceID() == backpackItem.GetInstanceID())) continue;

                foreach (var statKvp in backpackItem.GetBackpackItem().ItemStats.StatValues)
                {
                    if (ShouldIgnoreStat(statKvp, backpackItem.GetBackpackItem())) continue;

                    statValues[statKvp.Key] += statKvp.Value;
                }

                itemsAlreadyCalculated.Add(backpackItem);
            }

            StatsController.instance.ReloadBackpackPassives(statValues);
            StatsController.instance.SetTotalPlayerStats(statValues);
            return statValues;
        }

        private bool ShouldIgnoreStat(KeyValuePair<StatType, float> statKvp, BackpackItemSO backpackItem)
        {
            var isWeapon = backpackItem.ItemType == ItemType.Weapon;
            var shouldIgnoreWeaponStat = _weaponStatsToIgnoreWhenGettingBackpackPassives.Contains(statKvp.Key);

            var result = isWeapon && shouldIgnoreWeaponStat;
            return result;
        }

        internal void SellItem(MovableBackpackElement movableBackpackElement)
        {
            movableBackpackElement.SetSuccesfullDrop();
            MoneyController.instance.GainCoins(((BackpackItem)movableBackpackElement).GetBackpackItem().SellingPrice);
            Destroy(movableBackpackElement.gameObject);
            _vendorAnimator.SetTrigger("Activate");
            //ShopItemSold.Play(0);
            AudioController.instance.PlayAudioSourceAsSfxClip(ShopItemSold);
            ToggleBadDropZones(false);
        }
        #endregion


        public delegate void BackpackChangeStateHandler(object sender, BackpackChangeStateEventArgs e);
        public event BackpackChangeStateHandler OnChangeState;
    }
}