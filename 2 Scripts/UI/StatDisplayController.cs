using BackpackSurvivors.Backpack;
using BackpackSurvivors.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BackpackSurvivors.Shared.Enums;

public class StatDisplayController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] Animator _animator;
    [SerializeField] Image _button;
    [SerializeField] Sprite _openButtonSprite;
    [SerializeField] Sprite _closeButtonSprite;
    [SerializeField] ItemDataBase _dataBase;

    [Header("Aggresive Stats")]
    [SerializeField] TextMeshProUGUI _critChance;
    [SerializeField] TextMeshProUGUI _critDamage;
    [SerializeField] TextMeshProUGUI _damagePercentage;
    [SerializeField] TextMeshProUGUI _cooldownReduction;
    [SerializeField] TextMeshProUGUI _flagDamage;
    [SerializeField] TextMeshProUGUI _weaponRange;
    [SerializeField] TextMeshProUGUI _piercing;

    [Header("Defensive & Utility Stats")]
    [SerializeField] TextMeshProUGUI _healthPercentage;
    [SerializeField] TextMeshProUGUI _speedPercentage;
    [SerializeField] TextMeshProUGUI _damageTaken;
    [SerializeField] TextMeshProUGUI _armor;
    [SerializeField] TextMeshProUGUI _pickupRadius;
    [SerializeField] TextMeshProUGUI _Luck;
    [SerializeField] TextMeshProUGUI _enemyCount;

    private bool _isOpen;
    private bool _isReady;

    // Start is called before the first frame update
    void Start()
    {
        Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOpen)
        {
            SetValues();
        }
    }

    private void SetValues()
    {
        var stats = BackPackController.instance.CalculatePlayerPassivesFromBackpack();
        
        _critChance.SetText(_dataBase.GetCleanValue(stats[StatType.CritChancePercentage], StatType.CritChancePercentage));
        _critDamage.SetText(_dataBase.GetCleanValue(stats[StatType.CritMultiplier], StatType.CritMultiplier));
        _damagePercentage.SetText(_dataBase.GetCleanValue(stats[StatType.DamagePercentage], StatType.DamagePercentage));
        _cooldownReduction.SetText(_dataBase.GetCleanValue(stats[StatType.CooldownTime], StatType.CooldownTime));
        _flagDamage.SetText(_dataBase.GetCleanValue(stats[StatType.FlatDamage], StatType.FlatDamage));
        _weaponRange.SetText(_dataBase.GetCleanValue(stats[StatType.WeaponRange], StatType.WeaponRange));
        _piercing.SetText(_dataBase.GetCleanValue(stats[StatType.Piercing], StatType.Piercing));

        _healthPercentage.SetText(_dataBase.GetCleanValue(stats[StatType.Health], StatType.Health));
        _speedPercentage.SetText(_dataBase.GetCleanValue(stats[StatType.SpeedPercentage], StatType.SpeedPercentage));
        _damageTaken.SetText(_dataBase.GetCleanValue(stats[StatType.DamageReductionPercentage], StatType.DamageReductionPercentage));
        _armor.SetText(_dataBase.GetCleanValue(stats[StatType.Armor], StatType.Armor));
        _pickupRadius.SetText(_dataBase.GetCleanValue(stats[StatType.PickupRadiusPercentage], StatType.PickupRadiusPercentage));
        _Luck.SetText(_dataBase.GetCleanValue(stats[StatType.LuckPercentage], StatType.LuckPercentage));
        _enemyCount.SetText(_dataBase.GetCleanValue(stats[StatType.EnemyCount], StatType.EnemyCount));
    }

    public void Toggle()
    {
        if (_isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        _animator.SetBool("Showing", true);
        _button.sprite = _closeButtonSprite;
        _isOpen = true;
    }

    public void Close()
    {
        _animator.SetBool("Showing", false);
        _button.sprite = _openButtonSprite;
        _isOpen = false;
    }
}
