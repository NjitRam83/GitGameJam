using UnityEngine;

namespace BackpackSurvivors.Waves
{
    public class SpawnLocation : MonoBehaviour
    {
        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        private void Awake()
        {
            CalculateSpawnbounds();
        }

        private void CalculateSpawnbounds()
        {
            _minX = transform.position.x - (transform.localScale.x / 2);
            _maxX = transform.position.x + (transform.localScale.x / 2);
            _minY = transform.position.y - (transform.localScale.y / 2);
            _maxY = transform.position.y + (transform.localScale.y / 2);
        }

        public bool IsPositionWithinSpawnBounds(Vector2 position)
        {
            var positionWithinXBounds = position.x > _minX && position.x < _maxX;
            var positionWithinYBounds = position.y > _minY && position.y < _maxY;

            var positionWithinSpawnTransform = positionWithinXBounds && positionWithinYBounds;
            return positionWithinSpawnTransform;
        }

        public Vector2 GetRandomPositionWithinSpawnbounds()
        {
            var randomX = Random.Range(transform.position.x, transform.position.x + (transform.localScale.x / 2));
            var randomY = Random.Range(transform.position.y, transform.position.y + (transform.localScale.y / 2));
            var randomPosition = new Vector2(randomX, randomY);

            return randomPosition;
        }

        public Vector2 MovePositionToBeWithinSpawnbounds(Vector2 position)
        {
            position.x = Mathf.Max(position.x, _minX);
            position.x = Mathf.Min(position.x, _maxX);
            position.y = Mathf.Max(position.y, _minY);
            position.y = Mathf.Min(position.y, _maxY);

            //Debug.Log("Transform:");
            //Debug.Log($"Min: {_minX}, {_minY}");
            //Debug.Log($"Max: {_maxX}, {_maxY}");

            return position;
        }

    }
}
