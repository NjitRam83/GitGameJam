using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Combat.ScriptableObjects
{
    [CreateAssetMenu]
    public class ItemStatsSO : ScriptableObject
    {
        [SerializeField] public SerializableDictionaryBase<StatType, float> StatValues;

        public void Init()
        {
        }
    }
}