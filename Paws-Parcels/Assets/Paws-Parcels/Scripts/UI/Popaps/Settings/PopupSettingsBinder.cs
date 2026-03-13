using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Settings
{
    public class PopupSettingsBinder : WindowBinder<PopupSettingsViewModel>
    {
        [SerializeField] private byte _colorStrangeWhenCanSave;
        [SerializeField] private byte _colorStrangeWhenCouldntSave;
        
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _exitButtonBackground;
        [SerializeField] private Button _resetButton;

        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private TMP_InputField _masterVolumeInputField;
        
        [SerializeField] private Slider _sensivityPlayerSlider;
        [SerializeField] private TMP_InputField _sensivityPlayerInputField;

        [SerializeField] private Slider _sensivityCarSlider;
        [SerializeField] private TMP_InputField _sensivityCarInputField;

        [SerializeField] private TMP_Dropdown _languageDropdown;

        [SerializeField] private Button _saveButton;
        [SerializeField] private TMP_Text _saveText;
        
        private CompositeDisposable _disposables;
        
        private List<SettingsService.LocaleData> _locales;

        protected override void OnBind(PopupSettingsViewModel viewModel)
        {
            _disposables = new CompositeDisposable();

            _locales = viewModel.LanguageVariants;
            var localeNames = _locales.Select(x => x.Name).ToList();
            
            _languageDropdown.ClearOptions();
            _languageDropdown.AddOptions(localeNames);

            //buttons
            _disposables.Add(_exitButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestCloseChecker()));
            _disposables.Add(
                _exitButtonBackground.OnClickAsObservable().Subscribe(_ => viewModel.RequestCloseChecker()));
            _disposables.Add(_saveButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestSave()));
            _disposables.Add(_resetButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestReset()));

            //master volume
            _disposables.Add(viewModel.MasterVolume.Subscribe(v => _masterVolumeSlider.value = v));
            _disposables.Add(viewModel.MasterVolume.Subscribe(v => _masterVolumeInputField.text = v.ToString("F3")));
            _disposables.Add(_masterVolumeSlider.onValueChanged.AsObservable()
                .Subscribe(viewModel.RequestSetMasterVolumeBySlider));
            _disposables.Add(_masterVolumeInputField.onEndEdit.AsObservable().Subscribe(v =>
            {
                viewModel.RequestSetMasterVolumeByInputField(v);
                _masterVolumeInputField.text = viewModel.MasterVolume.CurrentValue.ToString("F3");
            }));
            
            //sensitivity player
            _disposables.Add(viewModel.SensitivityPlayer01.Subscribe(v => _sensivityPlayerSlider.SetValueWithoutNotify(v)));
            _disposables.Add(viewModel.SensitivityPlayer.Subscribe(v => _sensivityPlayerInputField.text = v.ToString("F3")));
            _disposables.Add(_sensivityPlayerSlider.onValueChanged.AsObservable()
                .Subscribe(viewModel.RequestSetSensitivityPlayerBySlider));
            _disposables.Add(_sensivityPlayerInputField.onEndEdit.AsObservable().Subscribe(v =>
            {
                viewModel.RequestSetSensitivityPlayerByInputField(v);
                _sensivityPlayerInputField.text = viewModel.SensitivityPlayer.CurrentValue.ToString("F3");
            }));
            
            //sensitivity car
            _disposables.Add(viewModel.SensitivityCar01.Subscribe(v => _sensivityCarSlider.SetValueWithoutNotify(v)));
            _disposables.Add(viewModel.SensitivityCar.Subscribe(v => _sensivityCarInputField.text = v.ToString("F3")));
            _disposables.Add(_sensivityCarSlider.onValueChanged.AsObservable()
                .Subscribe(viewModel.RequestSetSensitivityCarBySlider));
            _disposables.Add(_sensivityCarInputField.onEndEdit.AsObservable().Subscribe(v =>
            {
                viewModel.RequestSetSensitivityCarByInputField(v);
                _sensivityCarInputField.text = viewModel.SensitivityCar.CurrentValue.ToString("F3");
            }));
            
            //language
            _disposables.Add(viewModel.LanguageCode.Subscribe(v =>
            {
                _languageDropdown.value = _locales.IndexOf(_locales.First(x => x.Code == v));
            }));
            _disposables.Add(_languageDropdown.onValueChanged.AsObservable().Subscribe(index => viewModel.RequestSetLanguage(_locales[index].Code)));
            
            //button show
            _disposables.Add(viewModel.HasChanges.Subscribe(v => _saveText.color = v
                ? new Color(_saveText.color.r, _saveText.color.g, _saveText.color.b, (float)_colorStrangeWhenCanSave/255)
                : new Color(_saveText.color.r, _saveText.color.g, _saveText.color.b, (float)_colorStrangeWhenCouldntSave/255)));
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}