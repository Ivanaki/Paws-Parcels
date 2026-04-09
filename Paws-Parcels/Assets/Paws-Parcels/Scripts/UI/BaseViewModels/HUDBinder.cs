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
        [Header("Diary Found")]
        [SerializeField] private RectTransform _diaryFoundPageRectTransform;
        [SerializeField] private RectTransform _diaryFoundHiddenLeftPositionRectTransform;
        [SerializeField] private RectTransform _diaryFoundShownPositionRectTransform;
        [SerializeField] private float _diaryFoundMoveDuration = 0.35f;
        [SerializeField] private float _diaryFoundDelay = 1.25f;
        
        private readonly CompositeDisposable _disposables = new();
        private Sequence _diaryFoundSequence;

        public void Bind(HUDViewModel viewModel)
        {
            InitializeDiaryFoundWidget();

            _disposables.Add(viewModel.ActiveHud.Subscribe(value => { _parentAllHud.SetActive(value); }));

            _disposables.Add(viewModel.Money.Subscribe(value => { _moneyText.text = value.ToString(); }));

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
            _diaryFoundSequence?.Kill();
            _disposables.Dispose();
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
    }
}
