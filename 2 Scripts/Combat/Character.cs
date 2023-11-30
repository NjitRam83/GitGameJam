using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

[RequireComponent(typeof(Health))]
public abstract class Character : MonoBehaviour
{
    private Dictionary<StatType, float> _stats = new Dictionary<StatType, float>();
    private Dictionary<StatType, float> _previousStats = new Dictionary<StatType, float>();
    private CharacterType _characterType = CharacterType.Unknown;

    public CharacterType CharacterType => _characterType;

    public delegate void CharacterDiedHandler(object sender, CharacterDiedEventArgs e);
    public event CharacterDiedHandler OnCharacterDied;
    public EventHandler OnStatsUpdated;

    internal void SetCharacterType(CharacterType characterType)
    {
        _characterType = characterType;
    }

    public Health GetHealth()
    {
        return GetComponent<Health>();
    }

    public virtual void SetCharacterStats(Dictionary<StatType, float> stats)
    {
        _previousStats = _stats.Count == 0 ? stats : _stats;
        _stats = stats;
        OnStatsUpdated?.Invoke(this, null);
    }

    internal float GetStatValue(StatType statType)
    {
        if (_stats == null) return 0f;
        if (_stats.ContainsKey(statType) == false) return 0f;

        return _stats[statType];
    }

    internal float GetStatChange(StatType statType)
    {
        var currentValue = GetStatValue(statType);
        var previousValue = _previousStats[statType];    
        var change = currentValue - previousValue;
        return change;
    }

    public void DebugStats()
    {
        Debug.Log($"-- START {_characterType} STATS --");

        foreach (var stat in Enum.GetValues(typeof(StatType)))
        {
            Debug.Log($"{stat}: {GetStatValue((StatType)stat)}");
        }

        Debug.Log($"-- END {_characterType} STATS --");
    }

    internal void HookupCharacterDeath()
    {
        var health = GetComponent<Health>();
        health.OnDeath += CharacterDied;
    }

    public virtual void CharacterDied()
    {
        if (OnCharacterDied != null)
        {
            OnCharacterDied(this, new CharacterDiedEventArgs(this));
        }
    }

    internal void HookupCharacterHit()
    {
        var health = GetComponent<Health>();
        health.OnHit += CharacterHit;
    }

    public virtual void CharacterHit()
    {
    }
}
