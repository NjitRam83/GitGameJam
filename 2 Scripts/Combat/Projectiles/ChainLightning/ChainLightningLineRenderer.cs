using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Combat.Projectiles.ChainLightning
{
    public class ChainLightningLineRenderer : MonoBehaviour
    {

        [SerializeField] List<ChainLightningPoint> _linePoints;
        [SerializeField] LineRenderer _lineRenderer;

        Vector3[] _linePositions;

        public void AddPoint(ChainLightningPoint chainLightningPoint)
        {
            _linePoints.Add(chainLightningPoint);
        }

        // Update is called once per frame
        void Update()
        {
            _lineRenderer.positionCount = _linePoints.Count;
            _linePositions = new Vector3[_linePoints.Count];

            for (int i = 0; i < _linePoints.Count; i++)
            {
                _linePositions[i] = _linePoints[i].GetPosition();
            }

            _lineRenderer.SetPositions(_linePositions);
        }
    }
}