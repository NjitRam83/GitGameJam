using static BackpackSurvivors.Shared.Enums;
using UnityEngine;

namespace BackpackSurvivors.Items
{
    [CreateAssetMenu(fileName = "PassiveStatInformation", menuName = "Game/Passive Stat Information", order = 1)]
    public class PassiveStatInformation : ScriptableObject
    {
        [Header("Base")]
        [SerializeField] public StatType StatType;
        [SerializeField] public StatCalculationType StatCalculationType;

        [Header("Negative")]
        [SerializeField] public float NegativeWeight;
        [SerializeField] public float NegativeMinRange;
        [SerializeField] public float NegativeMaxRange;

        [Header("Positive")]
        [SerializeField] public float PostiveWeight;
        [SerializeField] public float PostiveMinRange;
        [SerializeField] public float PostiveMaxRange;
    }
}