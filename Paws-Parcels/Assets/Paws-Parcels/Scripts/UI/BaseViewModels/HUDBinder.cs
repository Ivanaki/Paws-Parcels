using System;
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
        
        private readonly CompositeDisposable _disposables = new();

        public void Bind(HUDViewModel viewModel)
        {
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
            }));

            _disposables.Add(viewModel.GameProxy.MissionTimerSeconds.Subscribe(seconds =>
            {
                _timerImage.fillAmount = seconds / viewModel.GameProxy.MissionTimerTotalSeconds.CurrentValue;
            }));
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
