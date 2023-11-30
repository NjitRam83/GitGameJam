using UnityEngine;

namespace BackpackSurvivors.Enemies.Components.Attacks
{
    [RequireComponent(typeof(Character))]
    public class EnemyAttack : MonoBehaviour
    {
        private float _baseCooldown;
        private float _currentCooldown;

        public Character GetDamageSourceCharacter()
        {
            var characterComponent = GetComponent<Character>();
            return characterComponent;
        }

        public void Init(float cooldown)
        {
            _baseCooldown = cooldown;
            _currentCooldown = cooldown;
        }

        internal void DecreaseCooldown(float cooldownDecrease)
        {
            _currentCooldown -= cooldownDecrease;
        }

        internal void ResetCooldown()
        {
            _currentCooldown = _baseCooldown;
        }

        internal bool CanAttackBasedOnCooldown()
        {
            return _currentCooldown <= 0;
        }
    }
}
