using System;
using DG.Tweening;
using ObservableCollections;
using R3;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class HUDBinder : MonoBehaviour
    {
        //[SerializeField] private DestinationsOnCanvasShower _destinationsOnCanvasShower;
        [SerializeField] private GameObject _parentAllHud;
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _currentQuestTitleText;
        [SerializeField] private TMP_Text _currentMissionTitleText;
        [SerializeField] private Image _timerImage;
        [SerializeField] private GameObject _clockImage;
        [Header("Money Change")]
        [SerializeField] private TMP_Text _moneyChangeText;
        [SerializeField] private RectTransform _moneyChangeStartPointRectTransform;
        [SerializeField] private RectTransform _moneyChangeEndPointRectTransform;
        [SerializeField] private Vector2 _moneyChangeStartOffset = new(-90f, 30f);
        [SerializeField] private Vector2 _moneyChangeEndOffset = new(0f, 0f);
        [SerializeField] private float _moneyChangeDuration = 0.85f;
        [SerializeField] private Color _moneyAddedColor = new(0.5254902f, 0.98039216f, 0.62352943f, 1f);
        [SerializeField] private Color _moneySpentColor = new(1f, 0.49019608f, 0.49019608f, 1f);
        [Header("Diary Found")]
        [SerializeField] private RectTransform _diaryFoundPageRectTransform;
        [SerializeField] private RectTransform _diaryFoundHiddenLeftPositionRectTransform;
        [SerializeField] private RectTransform _diaryFoundShownPositionRectTransform;
        [SerializeField] private float _diaryFoundMoveDuration = 0.35f;
        [SerializeField] private float _diaryFoundDelay = 1.25f;
        
        private readonly CompositeDisposable _disposables = new();
        private Sequence _moneyChangeSequence;
        private Sequence _diaryFoundSequence;
        private int _lastMoneyValue;
        private bool _hasMoneyValue;

        public void Bind(HUDViewModel viewModel)
        {
            InitializeMoneyChangeWidget();
            InitializeDiaryFoundWidget();

            _disposables.Add(viewModel.ActiveHud.Subscribe(value => { _parentAllHud.SetActive(value); }));

            _disposables.Add(viewModel.Money.Subscribe(HandleMoneyChanged));

            if (_currentQuestTitleText != null)
            {
                _disposables.Add(viewModel.CurrentQuestTitleText.Subscribe(value => { _currentQuestTitleText.text = value; }));
            }

            if (_currentMissionTitleText != null)
            {
                _disposables.Add(viewModel.CurrentMissionTitleText.Subscribe(value => { _currentMissionTitleText.text = value; }));
            }
            
            _disposables.Add(viewModel.GameProxy.HasMissionTimer.Subscribe(hasTimer =>
            {
                _timerImage.gameObject.SetActive(hasTimer);
                _clockImage.SetActive(hasTimer);
            }));

            _disposables.Add(viewModel.GameProxy.MissionTimerSeconds.Subscribe(seconds =>
            {
                _timerImage.fillAmount = seconds / viewModel.GameProxy.MissionTimerTotalSeconds.CurrentValue;
            }));

            if (_diaryFoundPageRectTransform != null)
            {
                _disposables.Add(viewModel.GameProxy.DiaryPages.ObserveAdd().Subscribe(e => ShowDiaryFoundAnimation()));
            }
        }

        private void OnDestroy()
        {
            _moneyChangeSequence?.Kill();
            _diaryFoundSequence?.Kill();
            _disposables.Dispose();
        }

        private void InitializeMoneyChangeWidget()
        {
            var moneyChangeText = ResolveMoneyChangeText();
            if (moneyChangeText == null)
            {
                return;
            }

            moneyChangeText.gameObject.SetActive(false);
            moneyChangeText.text = string.Empty;
            moneyChangeText.alpha = 0f;
        }

        private void InitializeDiaryFoundWidget()
        {
            var diaryFoundRectTransform = ResolveDiaryFoundRectTransform();
            if (diaryFoundRectTransform == null)
            {
                return;
            }

            if (diaryFoundRectTransform != null &&
                TryGetDiaryFoundAnchoredPosition(_diaryFoundHiddenLeftPositionRectTransform, out var hiddenPosition))
            {
                diaryFoundRectTransform.anchoredPosition = hiddenPosition;
            }

            diaryFoundRectTransform.gameObject.SetActive(false);
        }

        private RectTransform ResolveDiaryFoundRectTransform()
        {
            if (_diaryFoundPageRectTransform != null)
            {
                return _diaryFoundPageRectTransform;
            }

            return null;
        }

        private void HandleMoneyChanged(int value)
        {
            if (_moneyText != null)
            {
                _moneyText.text = value.ToString();
            }

            if (_hasMoneyValue)
            {
                var delta = value - _lastMoneyValue;
                if (delta != 0)
                {
                    ShowMoneyChangeAnimation(delta);
                }
            }

            _lastMoneyValue = value;
            _hasMoneyValue = true;
        }

        private TMP_Text ResolveMoneyChangeText()
        {
            if (_moneyChangeText != null)
            {
                return _moneyChangeText;
            }

            if (_moneyText == null)
            {
                return null;
            }

            var moneyChangeParentRectTransform = ResolveMoneyChangeParentRectTransform();
            if (moneyChangeParentRectTransform == null)
            {
                return null;
            }

            _moneyChangeText = Instantiate(_moneyText, moneyChangeParentRectTransform);
            _moneyChangeText.name = "MoneyChangeText";
            _moneyChangeText.raycastTarget = false;
            _moneyChangeText.text = string.Empty;

            var moneyChangeRectTransform = _moneyChangeText.rectTransform;
            moneyChangeRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            moneyChangeRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            moneyChangeRectTransform.localScale = Vector3.one;
            moneyChangeRectTransform.SetAsLastSibling();

            return _moneyChangeText;
        }

        private RectTransform ResolveMoneyChangeParentRectTransform()
        {
            if (_parentAllHud != null && _parentAllHud.TryGetComponent<RectTransform>(out var parentAllHudRectTransform))
            {
                return parentAllHudRectTransform;
            }

            if (_moneyText != null)
            {
                return _moneyText.rectTransform.parent as RectTransform;
            }

            return null;
        }

        private void ShowMoneyChangeAnimation(int delta)
        {
            var moneyChangeText = ResolveMoneyChangeText();
            var moneyChangeRectTransform = moneyChangeText?.rectTransform;
            var moneyChangeParentRectTransform = ResolveMoneyChangeParentRectTransform();
            if (moneyChangeText == null || moneyChangeRectTransform == null || moneyChangeParentRectTransform == null)
            {
                return;
            }

            if (TryGetMoneyChangeAnchoredPosition(_moneyChangeStartPointRectTransform, moneyChangeParentRectTransform,
                    _moneyChangeStartOffset, out var startPosition) == false ||
                TryGetMoneyChangeAnchoredPosition(_moneyChangeEndPointRectTransform, moneyChangeParentRectTransform,
                    _moneyChangeEndOffset, out var endPosition) == false)
            {
                return;
            }

            _moneyChangeSequence?.Kill();

            moneyChangeText.text = delta > 0 ? $"+{delta}" : delta.ToString();
            moneyChangeText.color = delta > 0 ? _moneyAddedColor : _moneySpentColor;
            moneyChangeText.alpha = 1f;
            moneyChangeText.gameObject.SetActive(true);
            moneyChangeRectTransform.anchoredPosition = startPosition;
            moneyChangeRectTransform.SetAsLastSibling();

            Sequence sequence = null;
            sequence = DOTween.Sequence()
                .Append(moneyChangeRectTransform.DOAnchorPos(endPosition, _moneyChangeDuration).SetEase(Ease.OutCubic))
                .Join(DOTween.To(() => moneyChangeText.alpha, value => moneyChangeText.alpha = value, 0f, _moneyChangeDuration))
                .SetUpdate(true)
                .OnKill(() =>
                {
                    moneyChangeText.gameObject.SetActive(false);
                    if (_moneyChangeSequence == sequence)
                    {
                        _moneyChangeSequence = null;
                    }
                });

            _moneyChangeSequence = sequence;
        }

        private void ShowDiaryFoundAnimation()
        {
            var diaryFoundRectTransform = ResolveDiaryFoundRectTransform();
            if (diaryFoundRectTransform == null)
            {
                return;
            }

            if (TryGetDiaryFoundAnchoredPosition(_diaryFoundHiddenLeftPositionRectTransform, out var hiddenPosition) == false ||
                TryGetDiaryFoundAnchoredPosition(_diaryFoundShownPositionRectTransform, out var shownPosition) == false)
            {
                _diaryFoundSequence?.Kill();
                diaryFoundRectTransform.gameObject.SetActive(true);
                _diaryFoundSequence = DOTween.Sequence()
                    .AppendInterval(_diaryFoundDelay)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        diaryFoundRectTransform.gameObject.SetActive(false);
                        _diaryFoundSequence = null;
                    });
                return;
            }

            _diaryFoundSequence?.Kill();

            diaryFoundRectTransform.gameObject.SetActive(true);
            diaryFoundRectTransform.anchoredPosition = hiddenPosition;

            _diaryFoundSequence = DOTween.Sequence()
                .Append(diaryFoundRectTransform.DOAnchorPos(shownPosition, _diaryFoundMoveDuration))
                .AppendInterval(_diaryFoundDelay)
                .Append(diaryFoundRectTransform.DOAnchorPos(hiddenPosition, _diaryFoundMoveDuration))
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    diaryFoundRectTransform.gameObject.SetActive(false);
                    _diaryFoundSequence = null;
                });
        }

        private static bool TryGetDiaryFoundAnchoredPosition(RectTransform targetRectTransform, out Vector2 anchoredPosition)
        {
            if (targetRectTransform != null)
            {
                anchoredPosition = targetRectTransform.anchoredPosition;
                return true;
            }

            anchoredPosition = default;
            return false;
        }

        private bool TryGetMoneyChangeAnchoredPosition(RectTransform targetRectTransform, RectTransform parentRectTransform,
            Vector2 fallbackOffset, out Vector2 anchoredPosition)
        {
            if (TryGetAnchoredPositionInParent(targetRectTransform, parentRectTransform, out anchoredPosition))
            {
                return true;
            }

            if (TryGetAnchoredPositionInParent(_moneyText?.rectTransform, parentRectTransform, out var moneyAnchoredPosition))
            {
                anchoredPosition = moneyAnchoredPosition + fallbackOffset;
                return true;
            }

            anchoredPosition = fallbackOffset;
            return false;
        }

        private static bool TryGetAnchoredPositionInParent(RectTransform targetRectTransform,
            RectTransform parentRectTransform, out Vector2 anchoredPosition)
        {
            if (targetRectTransform == null || parentRectTransform == null)
            {
                anchoredPosition = default;
                return false;
            }

            if (targetRectTransform.parent == parentRectTransform)
            {
                anchoredPosition = targetRectTransform.anchoredPosition;
                return true;
            }

            var canvas = parentRectTransform.GetComponentInParent<Canvas>();
            var camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, targetRectTransform.position);

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPoint, camera,
                out anchoredPosition);
        }
    }
}
