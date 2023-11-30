using UnityEngine;
using TMPro;
using System.Collections;
using System;

namespace BackpackSurvivors.UI
{
    public class NumberPopup : MonoBehaviour
    {
        [SerializeField] float _fadeOutTime;
        [SerializeField] float _minFontSize;
        [SerializeField] float _maxFontSize;
        [SerializeField] float _damageForMaxFontSize;

        private TextMeshProUGUI _numberText;
        private float _fadeOutTimeDone;

        public void Init(string text, Color color)
        {
            _numberText.text = text;
            _numberText.color = color;

            SetFontSizeBasedOnDamage();

            StartCoroutine(FadeOut());
        }

        private void SetFontSizeBasedOnDamage()
        {
            if (int.TryParse(_numberText.text, out int damage) == false) return;
            var fontSize = damage / _damageForMaxFontSize;
            fontSize += _minFontSize;
            fontSize = Mathf.Clamp(fontSize, _minFontSize, _maxFontSize);
            _numberText.fontSize = fontSize;
        }

        private void Awake()
        {
            _numberText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private IEnumerator FadeOut()
        {
            while (_fadeOutTimeDone < _fadeOutTime)
            {
                _fadeOutTimeDone += Time.deltaTime;
                var fadeOutPercentage = _fadeOutTimeDone / _fadeOutTime;
                var alpha = (1f - fadeOutPercentage);
                var color = new Color(_numberText.color.r, _numberText.color.g, _numberText.color.b, alpha);
                _numberText.color = color;

                yield return null;
            }

            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}