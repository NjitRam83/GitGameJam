using BackpackSurvivors.MainGame;
using System.Collections;
using System.Drawing;
using UnityEngine;

namespace BackpackSurvivors.Level
{
    internal class RaindropController : MonoBehaviour
    {
        [SerializeField] Player _player;
        [SerializeField] Raindrop _prefab;
        [SerializeField] GameObject _parent;
        [SerializeField] bool _enabled;
        [SerializeField] float _durationBetweenSpawns;
        [SerializeField] int _maxSpawns;
        int _currentSpawns;

        internal void RemovedDroplet()
        {
            _currentSpawns -= 1;
        }

        void Start()
        {
            StartCoroutine(StartSpawning());
        }

        public Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public Bounds CreateBoundsAroundPosition(Vector3 position, int horizontalOffset, int verticalOffset)
        {
            return new Bounds(position, new Vector3(horizontalOffset * 2, verticalOffset * 2));
        }

        IEnumerator StartSpawning()
        {
            while (_enabled)
            {
                if (_currentSpawns < _maxSpawns)
                {
                   

                    Raindrop drop = Instantiate(_prefab, _parent.transform);
                    drop.transform.position = RandomPointInBounds(CreateBoundsAroundPosition(_player.transform.position, 8, 5));
                    //todo: reposition
                    drop.Init(this);

                    _currentSpawns++;
                }
                yield return new WaitForSeconds(_durationBetweenSpawns);
            }
        }
    }
}
