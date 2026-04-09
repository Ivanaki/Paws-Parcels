using DG.Tweening;
using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.EditorSettings;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ButtonHoverTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const string SharedAudioHostName = "[UI_BUTTON_AUDIO]";

        [Header("Target")]
        [SerializeField] private RectTransform _target;
        [SerializeField] private Selectable _selectable;
        [SerializeField] private Button _button;

        [Header("Animation")]
        [SerializeField, Min(1f)] private float _hoverScale = 1.05f;
        [SerializeField, Min(1f)] private float _hoverPeakScale = 1.1f;
        [SerializeField, Min(0f)] private float _expandDuration = 0.16f;
        [SerializeField, Min(0f)] private float _settleDuration = 0.1f;
        [SerializeField, Min(0f)] private float _returnDuration = 0.14f;
        [SerializeField] private Ease _expandEase = Ease.OutBack;
        [SerializeField] private Ease _settleEase = Ease.OutQuad;
        [SerializeField] private Ease _returnEase = Ease.OutCubic;
        [SerializeField] private bool _ignoreTimeScale = true;

        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField, Range(0f, 1f)] private float _hoverVolume = 0.2f;
        [SerializeField, Range(0f, 1f)] private float _clickVolume = 0.2f;

        private static AudioSource _sharedAudioSource;

        private Vector3 _defaultScale;
        private Tween _activeTween;
        private bool _isInitialized;
        private EditorDataService _editorDataService;
        private SettingsStateProvider _settingsStateProvider;
        private EditorSettingsSO _editorSettings;

        [Inject]
        private void Construct(EditorDataService editorDataService, SettingsStateProvider settingsStateProvider)
        {
            _editorDataService = editorDataService;
            _settingsStateProvider = settingsStateProvider;
        }

        private void Reset()
        {
            _target = transform as RectTransform;
            _selectable = GetComponent<Selectable>();
            _button = GetComponent<Button>();
            _audioSource = null;
        }

        private void Awake()
        {
            if (_target == null)
                _target = transform as RectTransform;

            if (_selectable == null)
                _selectable = GetComponent<Selectable>();

            if (_button == null)
                _button = GetComponent<Button>();

            _audioSource = GetOrCreateAudioSource();

            CacheDefaultScale();
        }

        private void OnEnable()
        {
            if (_button != null)
                _button.onClick.AddListener(PlayClickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CanAnimate() == false)
                return;

            PlayHoverSound();
            AnimateHover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_target == null)
                return;

            AnimateReturn();
        }

        private void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(PlayClickSound);

            KillActiveTween();
            RestoreDefaultScale();
        }

        private void OnDestroy()
        {
            KillActiveTween();
        }

        private bool CanAnimate()
        {
            return _target != null &&
                   (_selectable == null || _selectable.IsInteractable());
        }

        private void AnimateHover()
        {
            KillActiveTween();

            var peakScale = _defaultScale * Mathf.Max(_hoverPeakScale, _hoverScale);
            var settledScale = _defaultScale * _hoverScale;

            _activeTween = DOTween.Sequence()
                .Append(_target.DOScale(peakScale, _expandDuration).SetEase(_expandEase))
                .Append(_target.DOScale(settledScale, _settleDuration).SetEase(_settleEase))
                .SetUpdate(_ignoreTimeScale);
        }

        private void AnimateReturn()
        {
            KillActiveTween();

            _activeTween = _target
                .DOScale(_defaultScale, _returnDuration)
                .SetEase(_returnEase)
                .SetUpdate(_ignoreTimeScale);
        }

        private void PlayHoverSound()
        {
            var hoverClip = ResolveHoverClip();
            var hoverVolume = ResolveHoverVolume();

            if (_audioSource == null || hoverClip == null)
                return;

            _audioSource.PlayOneShot(hoverClip, hoverVolume);
        }

        private void PlayClickSound()
        {
            var clickClip = ResolveClickClip();
            var clickVolume = ResolveClickVolume();

            if (_audioSource == null || clickClip == null)
                return;

            _audioSource.PlayOneShot(clickClip, clickVolume);
        }

        private AudioClip ResolveHoverClip()
        {
            return GetEditorSettings()?.ButtonHoverSound;
        }

        private float ResolveHoverVolume()
        {
            return _hoverVolume * ResolveMasterVolume();
        }

        private AudioClip ResolveClickClip()
        {
            return GetEditorSettings()?.ButtonClickSound;
        }

        private float ResolveClickVolume()
        {
            return _clickVolume * ResolveMasterVolume();
        }

        private float ResolveMasterVolume()
        {
            return _settingsStateProvider?.SettingsProxy?.MasterVolume?.CurrentValue ?? 1f;
        }

        private EditorSettingsSO GetEditorSettings()
        {
            var editorSettings = _editorDataService?.EditorSettings ?? _editorSettings;

            if (editorSettings == null)
            {
                _editorSettings = Resources.Load<EditorSettingsSO>("Editor/EditorSettingsSO");
                editorSettings = _editorSettings;
            }

            return editorSettings;
        }

        private AudioSource GetOrCreateAudioSource()
        {
            if (_sharedAudioSource != null)
                return _sharedAudioSource;

            var audioHost = GameObject.Find(SharedAudioHostName);

            if (audioHost == null)
            {
                audioHost = new GameObject(SharedAudioHostName);
                DontDestroyOnLoad(audioHost);
            }

            _sharedAudioSource = audioHost.GetComponent<AudioSource>();

            if (_sharedAudioSource == null)
                _sharedAudioSource = audioHost.AddComponent<AudioSource>();

            _sharedAudioSource.playOnAwake = false;
            _sharedAudioSource.loop = false;
            _sharedAudioSource.spatialBlend = 0f;

            return _sharedAudioSource;
        }

        private void RestoreDefaultScale()
        {
            if (_target == null)
                return;

            if (_isInitialized == false)
                CacheDefaultScale();

            _target.localScale = _defaultScale;
        }

        private void CacheDefaultScale()
        {
            if (_target == null)
                return;

            _defaultScale = _target.localScale;
            _isInitialized = true;
        }

        private void KillActiveTween()
        {
            if (_activeTween == null)
                return;

            if (_activeTween.IsActive())
                _activeTween.Kill();

            _activeTween = null;
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_hoverPeakScale < _hoverScale)
                _hoverPeakScale = _hoverScale;

            if (_target == null)
                _target = transform as RectTransform;

            if (_selectable == null)
                _selectable = GetComponent<Selectable>();

            if (_button == null)
                _button = GetComponent<Button>();
        }
        #endif
    }
}
