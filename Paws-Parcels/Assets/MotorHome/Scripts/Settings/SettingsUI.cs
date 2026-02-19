using System;
using System.Collections.Generic;
using System.Linq;
using MotorHome.Scripts.Saves;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace MotorHome.Scripts
{
    public class SettingsUI : MonoBehaviour
    {
        private const float PlayerAimSliderMultiplayer = 3f;
        private const float CarAimSliderMultiplayer = 3f;

        [Header("Volume")]
        //[SerializeField] private TMP_Text _textVolume;
        [SerializeField] private Slider _sliderVolume;
        [SerializeField] private TMP_InputField _inputVolume;

        [Header("Player Aim")]
        //[SerializeField] private TMP_Text _textPlayerAim;
        [SerializeField] private Slider _sliderPlayerAim;
        [SerializeField] private TMP_InputField _inputPlayerAim;

        [Header("Car Aim")]
        //[SerializeField] private TMP_Text _textCarAim;
        [SerializeField] private Slider _sliderCarAim;
        [SerializeField] private TMP_InputField _inputCarAim;
        
        [Header("ChangeLanguage")]
        [SerializeField] private TMP_Dropdown _languageDropdown;
        
        [Header("Saves")] 
        [SerializeField] private Button _saveButton;
        [SerializeField] private TMP_Text _saveText;
        [SerializeField] private Button _resetButton;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _exitButton;
        
        [Header("CheckChanges")]
        [SerializeField] private GameObject _checkChangesUI;
        [SerializeField] private Button _checkChangesSaveButton;
        [SerializeField] private Button _checkChangesCancelButton;
        
        [Header("Colors")]
        [SerializeField] private Color _saveColorHide;
        [SerializeField] private Color _saveColorShow;

        
        public ReactiveProperty<float> Volume { get; } = new(1f);
        public ReactiveProperty<float> PlayerAim { get; } = new(1f);
        public ReactiveProperty<float> CarAim { get; } = new(1f);
        public ReactiveProperty<LanguageEnum> LanguageProperty = new();
        
        //public ReadOnlyReactiveProperty<LanguageEnum> Language => _languageProperty;
        public Observable<Unit> SaveButtonPressed => _saveButtonPressed;
        public Observable<Unit> ResetButtonPressed => _resetButtonPressed;
        public Observable<Unit> ExitButtonPressed => _exitButtonPressed;
        
        
        private readonly Subject<Unit> _saveButtonPressed = new();
        private readonly Subject<Unit> _resetButtonPressed = new();
        private readonly Subject<Unit> _exitButtonPressed = new();
        
        
        private List<(LanguageEnum, string)> _languages;
        
        public void SetLanguageOptions(List<(LanguageEnum, string)> languages, LanguageEnum firstValue)
        {
            LanguageProperty.Value = firstValue;
            _languages = languages;
            LanguageSet();
        }
        
        private void LanguageSet()
        {
            _languageDropdown.ClearOptions();
            _languageDropdown.AddOptions(_languages.Select(x => x.Item2).ToList());

            LanguageProperty.Subscribe(value =>
            {
                _languageDropdown.value = _languages.IndexOf(_languages.FirstOrDefault(x => x.Item1 == value));
            });
            
            _languageDropdown.onValueChanged.AddListener(value =>
            {
                LanguageProperty.Value = _languages[value].Item1;
            });
        }
        
        private void VolumeSet()
        {
            Volume.Subscribe(value =>
            {
                value = Mathf.Clamp(value, 0f, 1f);
                _sliderVolume.value = value;
                _inputVolume.text = value.ToString("F3");
            });
            
            _inputVolume.onEndEdit.AddListener(value =>
            {
                var a = float.Parse(value);
                Volume.Value = Mathf.Clamp(a, 0f, 1f);;
            });
            
            _sliderVolume.onValueChanged.AddListener(value =>
            {
                Volume.Value = value;
            });
        }
        
        private void PlayerAimSet()
        {
            PlayerAim.Subscribe(value =>
            {
                _sliderPlayerAim.value = Mathf.Clamp(value/PlayerAimSliderMultiplayer, 0f, 1f);
                _inputPlayerAim.text = Mathf.Clamp(value, 0f, 10f).ToString("F3");
            });
            
            _inputPlayerAim.onEndEdit.AddListener(value =>
            {
                var a = float.Parse(value);
                PlayerAim.Value = Mathf.Clamp(a, 0f, 10f);
            });
            
            _sliderPlayerAim.onValueChanged.AddListener(value =>
            {
                PlayerAim.Value = value * PlayerAimSliderMultiplayer;
            });
        }
        
        private void CarAimSet()
        {
            CarAim.Subscribe(value =>
            {
                _sliderCarAim.value = Mathf.Clamp(value/CarAimSliderMultiplayer, 0f, 1f);
                _inputCarAim.text = Mathf.Clamp(value, 0f, 10f).ToString("F3");
            });
            
            _inputCarAim.onEndEdit.AddListener(value =>
            {
                var a = float.Parse(value);
                CarAim.Value = Mathf.Clamp(a, 0f, 10f);
            });
            
            _sliderCarAim.onValueChanged.AddListener(value =>
            {
                CarAim.Value = value * CarAimSliderMultiplayer;
            });
        }
        
        
        
        private void Awake()
        {
            _checkChangesUI.SetActive(false);
            
            _saveButton.onClick.AddListener(() => _saveButtonPressed.OnNext(Unit.Default));
            _resetButton.onClick.AddListener(() => _resetButtonPressed.OnNext(Unit.Default));
            
            _exitButton.onClick.AddListener(() => _exitButtonPressed.OnNext(Unit.Default));
            
           VolumeSet();
           PlayerAimSet();
           CarAimSet();
           //LanguageSet();
           Hide();
        }

        public void Show()
        {
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        public void UpdateSaveButtonChanges(bool haveChanges)
        {
            //print(haveChanges);
            _saveText.color = haveChanges ? _saveColorShow : _saveColorHide;
        }

        public Observable<bool> NeedSaveChanges()
        {
            var returnObservable = new Subject<bool>();
            _checkChangesUI.gameObject.SetActive(true);

            UnityAction saveHandler = null;
            UnityAction cancelHandler = null;

            saveHandler = () =>
            {
                returnObservable.OnNext(true);
                Cleanup();
            };

            cancelHandler = () =>
            {
                returnObservable.OnNext(false);
                Cleanup();
            };

            _checkChangesSaveButton.onClick.AddListener(saveHandler);
            _checkChangesCancelButton.onClick.AddListener(cancelHandler);

            void Cleanup()
            {
                _checkChangesSaveButton.onClick.RemoveListener(saveHandler);
                _checkChangesCancelButton.onClick.RemoveListener(cancelHandler);

                _checkChangesUI.gameObject.SetActive(false);
                returnObservable.OnCompleted();
            }

            return returnObservable;
        }
        
        public void CloseCheckChangesUI()
        {
            _checkChangesUI.gameObject.SetActive(false);
        }
    }
}