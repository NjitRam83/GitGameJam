using UnityEngine;

namespace BackpackSurvivors.Combat.ScriptableObjects
{
    public class ScalingSO<T> : ScriptableObject
    {
        [SerializeField] public T ScalingType;
        [SerializeField] public float Scale;
    }
}
