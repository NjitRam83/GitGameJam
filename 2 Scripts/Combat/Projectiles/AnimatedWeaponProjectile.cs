using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Combat.Projectiles
{
    public class AnimatedWeaponProjectile : WeaponProjectile
    {
        [SerializeField] Animator _animator;
        public override void ActivateWeaponProjectile(Transform sourceTransform, Vector2 targetPosition, float speed, float maxRange)
        {
            var newTargetPosition = GetTargetAtMaxRange(targetPosition, maxRange);
            base.ActivateWeaponProjectile(sourceTransform, newTargetPosition, speed, maxRange);
            _animator.SetTrigger("Spawn");
            StartCoroutine(StartFlying());
        }


        private Vector2 GetTargetAtMaxRange(Vector2 targetPosition, float range)
        {
            var targetToCurrentOffset = targetPosition - (Vector2)transform.position;
            var distanceToTarget = Vector2.Distance(targetPosition, transform.position);
            var rangeMultiplier = range / distanceToTarget;
            var maxRangeToCurrentOffset = rangeMultiplier * targetToCurrentOffset;
            var targetAtMaxRange = (Vector2)transform.position + maxRangeToCurrentOffset;
            return targetAtMaxRange;
        }

        private IEnumerator StartFlying()
        {
            yield return new WaitForSeconds(0.3f);
            _animator.SetBool("Going", true);
        }


        public override void DestroyIfTargetReached()
        {
            if (_destroyOnTargetReached == false) return;
            if (double.IsNaN(_targetPosition.x) || double.IsNaN(_targetPosition.y))
            {
                StartCoroutine(DoDestroy());
            }
            if (Vector2.Distance(transform.position, _targetPosition) <= float.Epsilon)
            {
                StartCoroutine(DoDestroy());
            }
        }

        public override void DestroyIfOutOfRange()
        {
            if (_destroyOnRangeReached == false) return;

            var distanceTravelled = Vector2.Distance(transform.position, _startPosition);
            if (distanceTravelled > _maxRange)
            {
                StartCoroutine(DoDestroy());
            }
        }




        //public override void DestroyIfTargetReached()
        //{
        //    if (_destroyOnTargetReached == false) return;
        //    if (Vector2.Distance(transform.position, _targetPosition) <= float.Epsilon)
        //    {
        //        StartCoroutine(DoDestroy());
        //    }
        //}

        //public override void DestroyIfOutOfRange()
        //{
        //    if (_destroyOnRangeReached == false) return;
        //    var distanceTravelled = Vector2.Distance(transform.position, _startPosition);
        //    if (distanceTravelled > _maxRange)
        //    {
        //        StartCoroutine(DoDestroy());
        //    }
        //}

        private IEnumerator DoDestroy()
        {
            _animator.SetBool("Going", false);
            _animator.SetBool("OnHit", true);
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
    }
}