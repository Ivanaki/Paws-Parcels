using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScreenFade : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeDuration = 0.4f;

        private bool _isFading;

        private void Awake()
        {
            var color = _fadeImage.color;
            color.a = 0f;
            _fadeImage.color = color;
            _fadeImage.raycastTarget = false;
            _fadeImage.enabled = false;
        }

        public void SetFadeDuration(float duration)
        {
            _fadeDuration = duration;
        }

        public void FadeInOut(Action onFullyFaded)
        {
            if (_isFading) return;
            StartCoroutine(FadeSequence(onFullyFaded));
        }

        private IEnumerator FadeSequence(Action onFullyFaded)
        {
            _isFading = true;
            _fadeImage.enabled = true;
            _fadeImage.raycastTarget = true;

            Time.timeScale = 0f;

            yield return _fadeImage.DOFade(1f, _fadeDuration).SetUpdate(true).WaitForCompletion();

            onFullyFaded?.Invoke();

            yield return _fadeImage.DOFade(0f, _fadeDuration).SetUpdate(true).WaitForCompletion();

            Time.timeScale = 1f;

            _fadeImage.raycastTarget = false;
            _fadeImage.enabled = false;
            _isFading = false;
        }
    }
}
