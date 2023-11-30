using UnityEngine;

namespace BackpackSurvivors.UI
{
    public class ArrowPointingAtTargetScript : MonoBehaviour
    {
        public GameObject ObjectToTarget;
        public GameObject Player;
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] SpriteRenderer _onTargetSprite;
        [SerializeField] float _rangeToTrigger;

        private bool _onTarget = false;

        // Start is called before the first frame update
        void Start()
        {
            isReady = true;
        }
        private bool isReady = false;
        void FixedUpdate()
        {
            if (isReady)
            {
                float distance = Vector3.Distance(ObjectToTarget.transform.position, Player.transform.position);
                if (distance > _rangeToTrigger)
                {
                    if (_onTarget)
                    {
                        _spriteRenderer.enabled = true;
                        _onTargetSprite.enabled = false;
                        _onTarget = false;
                    }

                    Vector3 direction = (ObjectToTarget.transform.position - Player.transform.position).normalized;
                    Vector3 neutralDir = transform.up;
                    float angle = Vector3.SignedAngle(neutralDir, direction, Vector3.forward) + 90f;
                    direction = Quaternion.AngleAxis(angle, Vector3.forward) * neutralDir;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                    transform.position = Player.transform.position;
                }
                else
                {
                    if (!_onTarget)
                    {
                        _spriteRenderer.enabled = false;
                        _onTargetSprite.enabled = true;
                        _onTarget = true;
                    }
                }
            }
        }
    }
}