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

        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private TMP_InputField _musicVolumeInputField;
        
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
            BindVolumeControl(viewModel.MasterVolume, _masterVolumeSlider, _masterVolumeInputField,
                viewModel.RequestSetMasterVolumeBySlider, viewModel.RequestSetMasterVolumeByInputField);

            //music volume
            BindVolumeControl(viewModel.MusicVolume, _musicVolumeSlider, _musicVolumeInputField,
                viewModel.RequestSetMusicVolumeBySlider, viewModel.RequestSetMusicVolumeByInputField);
            
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

        private void BindVolumeControl(ReadOnlyReactiveProperty<float> volumeProperty, Slider slider,
            TMP_InputField inputField, Action<float> onSliderChanged, Action<string> onInputFieldChanged)
        {
            if (slider != null)
            {
                _disposables.Add(volumeProperty.Subscribe(v => slider.SetValueWithoutNotify(v)));
                _disposables.Add(slider.onValueChanged.AsObservable().Subscribe(onSliderChanged));
            }

            if (inputField != null)
            {
                _disposables.Add(volumeProperty.Subscribe(v => inputField.SetTextWithoutNotify(v.ToString("F3"))));
                _disposables.Add(inputField.onEndEdit.AsObservable().Subscribe(v =>
                {
                    onInputFieldChanged(v);
                    inputField.SetTextWithoutNotify(volumeProperty.CurrentValue.ToString("F3"));
                }));
            }
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
