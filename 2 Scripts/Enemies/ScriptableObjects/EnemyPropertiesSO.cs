using UnityEngine;

namespace BackpackSurvivors.Enemies.ScriptableObjects
{
    [CreateAssetMenu]
    public class EnemyPropertiesSO : ScriptableObject
    {
        [SerializeField] public float Hp;
        [SerializeField] public float MinimumMoveSpeed;
        [SerializeField] public float MaximumMoveSpeed;
        [SerializeField] public float MinDamageOnTouch;
        [SerializeField] public float MaxDamageOnTouch;
        [SerializeField] public float CritChance; 
        [SerializeField] public float CritMultiplier;
        [SerializeField] public int CoinsDroppedOnDeath;
        [SerializeField] public AudioClip AudioOnHit;
        [SerializeField] public AudioClip AudioOnDeath;
        [SerializeField] public bool IsBoss;
        [SerializeField] public bool IsMiniBoss;
    }
}
