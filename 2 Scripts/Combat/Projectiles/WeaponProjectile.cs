using BackpackSurvivors.Combat.Movement;
using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Combat.Projectiles
{
    [RequireComponent(typeof(IProjectileMovement))]
    public class WeaponProjectile : MonoBehaviour
    {
        [SerializeField] internal bool _destroyOnTargetReached;
        [SerializeField] internal bool _destroyOnRangeReached;
        [SerializeField] bool _destroyIfNotMoved;
        [SerializeField] SpriteRenderer _spriteToRotate;
        [SerializeField] BoxCollider2D _boxColider;
        [SerializeField] CircleCollider2D _circleColider;
        [SerializeField] float _timeBeforeDestroy;
        [SerializeField] float _speed = 20f;
        internal Vector2 _startPosition;
        internal Vector2 _targetPosition;
        private Vector2 _previousPosition;
        internal float _maxRange;
        private float _projectileImageRotation;
        private DamageStats _damageStats;
        private IProjectileMovement _projectileMovement;
        private bool _beingDestroyed;

        public DamageStats DamageStats => _damageStats;

        public virtual void ActivateWeaponProjectile(Transform sourceTransform, Vector2 targetPosition, float speed, float maxRange)
        {
            _boxColider = GetComponent<BoxCollider2D>();
            _circleColider = GetComponent<CircleCollider2D>();
            _startPosition = transform.position;
            _targetPosition = targetPosition;
            //_speed = speed;
            _maxRange = maxRange;

            RotateProjectile(targetPosition);
        }

        private void RotateProjectile(Vector2 targetPosition)
        {
            _projectileMovement = GetComponent<IProjectileMovement>();
            if (_projectileMovement.ShouldRotateToFaceTarget() == false) return;

            Vector3 relative = transform.InverseTransformPoint(targetPosition);
            var angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            angle += _projectileImageRotation;
            if (double.IsNaN(-angle))
            {
                _spriteToRotate.transform.Rotate(0, 0, 0);
            }
            else
            {
                _spriteToRotate.transform.Rotate(0, 0, -angle);
            }
        }

        private void Awake()
        {
            _damageStats = GetComponent<DamageStats>();
            _projectileMovement = GetComponent<IProjectileMovement>();
        }

        private void Update()
        {
            DoUpdate();
        }

        public virtual void DoUpdate()
        {
            _previousPosition = transform.position;
            var newPosition = _projectileMovement.GetNewPosition(transform.position, _targetPosition, _speed * Time.deltaTime);
            if (!double.IsNaN(newPosition.x) && !double.IsNaN(newPosition.y))
            {
                transform.position = newPosition;
            }
            
            DestroyIfOutOfRange();
            DestroyIfTargetReached();
            DestroyIfNotMoved();
        }

        private void DestroyIfNotMoved()
        {
            if (_beingDestroyed) return;

            if (_destroyIfNotMoved == false) return;

            if ((Vector2)transform.position == _previousPosition)
            {
                StartCoroutine(StartDestroy());
            }
        }

        public virtual void DestroyIfTargetReached()
        {
            if (_beingDestroyed) return;

            if (_destroyOnTargetReached == false) return;
            if (double.IsNaN(_targetPosition.x) || double.IsNaN(_targetPosition.y))
            {
                StartCoroutine(StartDestroy());
            }
            if (Vector2.Distance(transform.position, _targetPosition) <= float.Epsilon)
            {
                StartCoroutine(StartDestroy());
            }
        }

        public virtual void DestroyIfOutOfRange()
        {
            if (_beingDestroyed) return;

            if (_destroyOnRangeReached == false) return;

            var distanceTravelled = Vector2.Distance(transform.position, _startPosition);
            if (distanceTravelled > _maxRange)
            {
                StartCoroutine(StartDestroy());
            }
        }

        private IEnumerator StartDestroy()
        {
            _beingDestroyed = true;

            if (_boxColider != null)
            {
                _boxColider.enabled = false;
            }
            if (_circleColider != null)
            {
                _circleColider.enabled = false;
            }
            if (_spriteToRotate != null)
            {
                _spriteToRotate.enabled = false;
            }

            yield return new WaitForSeconds(_timeBeforeDestroy);

            DoDestroy();
        }

        private void DoDestroy()
        {
            Destroy(gameObject);
        }
    }
}