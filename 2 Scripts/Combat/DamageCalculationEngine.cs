using BackpackSurvivors.Combat;
using static BackpackSurvivors.Shared.Enums;
using Random = UnityEngine.Random;


public static class DamageCalculationEngine
{
    public static float CalculateDamage(
        DamageStats damageStats,
        Character attackSourceCharacter,
        Character attackTargetCharacter,
        out bool wasCrit)
    {
        var health = attackTargetCharacter.GetHealth();
        if (health != null && health.Invulnerable)
        {
            wasCrit = false;
            return 0f;
        }


        wasCrit = CalculateIfCrit(damageStats, attackSourceCharacter, attackTargetCharacter);
        float damageDone = CalculateTotalDamage(damageStats, attackSourceCharacter, attackTargetCharacter, wasCrit);

        if (damageDone < 0)
        {
            damageDone = 0;
        }

        return damageDone;
    }
    private static bool CalculateIfCrit(
        DamageStats damageStats,
        Character attackSourceCharacter,
        Character attackTargetCharacter)
    {
        float fullCritChance = damageStats.CritChance;
        fullCritChance += attackSourceCharacter.GetStatValue(StatType.CritChancePercentage);

        bool critted = fullCritChance >= Random.Range(0f, 1f);
        return critted;
    }

    private static float CalculateTotalDamage(
        DamageStats damageStats,
        Character attackSourceCharacter,
        Character attackTargetCharacter,
        bool critted)
    {
        float damageDone = CalculateDamageBonusses(damageStats, attackSourceCharacter, critted);

        // Reduce damage calculations

        // reduce with armor
        float targetArmor = attackTargetCharacter.GetStatValue(StatType.Armor);
        damageDone = damageDone - targetArmor;

        // reduce with damage reduction
        float damageReduction = attackTargetCharacter.GetStatValue(StatType.DamageReductionPercentage);
        damageDone = damageDone * (1 - damageReduction);

        return damageDone;
    }

    private static float CalculateDamageBonusses(DamageStats damageStats, Character attackSourceCharacter, bool critted)
    {
        // calculate the base damage
        float damageDone = CalculateMinMaxDamage(damageStats, attackSourceCharacter);
        //Je hebt gelijk Martijn. TODO: hou op line 73

        // Add Global Damage Bonus
        damageDone = CalculateGlobalDamageBonus(attackSourceCharacter, damageDone);

        damageDone = CalculateCritDamageBonus(damageStats, attackSourceCharacter, critted, damageDone);
        return damageDone;
    }

    private static float CalculateCritDamageBonus(DamageStats damageStats, Character attackSourceCharacter, bool critted, float damageDone)
    {
        if (critted)
        {
            float critMultiplier = 1 +
                damageStats.CritMultiplier + 
                attackSourceCharacter.GetStatValue(StatType.CritMultiplier);  

            damageDone = damageDone * critMultiplier;
        }

        return damageDone;
    }
    private static float CalculateGlobalDamageBonus(Character attackSourceCharacter, float damageDone)
    {
        float globalDamageBonus = attackSourceCharacter.GetStatValue(StatType.DamagePercentage);
        damageDone = damageDone * (1 + globalDamageBonus);
        return damageDone;
    }
    private static float CalculateMinMaxDamage(DamageStats damageStats, Character attackSourceCharacter)
    {
        float baseMinDamage = damageStats.MinDamage;
        float baseMaxDamage = damageStats.MaxDamage;

        float damageAddition = attackSourceCharacter.GetStatValue(StatType.FlatDamage);

        // Add minDamage and maxDamage bonuses
        baseMinDamage += damageAddition;
        baseMaxDamage += damageAddition;
        if (baseMaxDamage < baseMinDamage)
        {
            baseMaxDamage = baseMinDamage;
        }
        float damageDone = Random.Range(baseMinDamage, baseMaxDamage);
        return damageDone;
    }
}