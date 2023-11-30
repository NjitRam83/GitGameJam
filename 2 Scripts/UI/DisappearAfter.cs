using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI
{
    internal class DisappearAfter : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Image _image;
        [SerializeField] float _alphaReduction;
        [SerializeField] float _alphaReductionDelay;
        [SerializeField] float _delayBeforeStart;
        private void Start()
        {
            if (_spriteRenderer != null)
            {
                StartCoroutine(StartHidingSpriteRenderer());
            }
        }

        private IEnumerator StartHidingSpriteRenderer()
        {
            yield return new WaitForSeconds(_delayBeforeStart);
            Color baseColor = _spriteRenderer.color;
            float alpha = baseColor.a;
            while (_spriteRenderer.color.a > 0)
            {
                alpha = alpha - _alphaReduction;
                _spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);   
                yield return new WaitForSeconds(_alphaReductionDelay);
            }
            Destroy(gameObject);
        }
    }
}
