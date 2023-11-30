using UnityEngine;

namespace BackpackSurvivors.Combat
{
    public class DamageStats : MonoBehaviour
    {
        [SerializeField] public float CritChance;
        [SerializeField] public float CritMultiplier;
        [SerializeField] public float MinDamage;
        [SerializeField] public float MaxDamage;
        [SerializeField] public int Piercing;
    }
}
