using Extentions;
using Paws_Parcels.Scripts.Player;
using Quests;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Services
{
    public class InteractableUIObject : UiObject
    {
        [SerializeField] private Image _smallDot;
        [SerializeField] private Image _bigCircle;
        [SerializeField] private Image _bigDot;
        
        //[SerializeField] private Image _image;
        [SerializeField] private Image _handShowImage;
        [SerializeField] private TMP_Text _tmpText;

        
        /*[SerializeField] private Sprite _spriteBase;
        [SerializeField] private Sprite _spriteActiveQuest;*/

        [Inject] private LocalizationService _localizationService;
        
        private CompositeDisposable _disposable;
        
        public override void Construct(IShowableInUI showableInUIObject)
        {
            var interactableObject = showableInUIObject as InteractableObject;
            
            if (interactableObject == null)
            {
                Debug.LogWarning("InteractableUIObject has no InteractableObject");
                return;
            }
            
            _disposable = new CompositeDisposable();

            _disposable.Add(_localizationService.CurrentLocale.Subscribe(_ => 
                UpdateShowText(interactableObject)));

            _disposable.Add(interactableObject.UIShowTextCode.Subscribe(_ => 
                UpdateShowText(interactableObject)));
            
            _disposable.Add(interactableObject.IsQuestSelect.Subscribe(v =>
            {
                UpdateColor(interactableObject);
            }));

            _disposable.Add(interactableObject.CanInteract.Subscribe(v =>
            {
                _handShowImage.gameObject.SetActive(v);
                _tmpText.gameObject.SetActive(v);
            }));
            
            _disposable.Add(interactableObject.IsQuestActive.Subscribe(v =>
            {
                _bigCircle.gameObject.SetActive(v);
                _smallDot.gameObject.SetActive(v);
                _bigDot.gameObject.SetActive(!v);
                UpdateColor(interactableObject);
                //_image.rectTransform.sizeDelta = v ? new Vector2(68, 68) : new Vector2(25, 25);
                //print(transform.GetFullPath());
            }));
        }

        private void UpdateColor(InteractableObject interactableObject)
        {
            var activeYellow = interactableObject.IsQuestSelect.CurrentValue &&
                               interactableObject.IsQuestActive.CurrentValue;
            _smallDot.color = activeYellow ? Color.yellow : Color.white;
            _bigDot.color = activeYellow ? Color.yellow : Color.white;
            _bigCircle.color = activeYellow ? Color.yellow : Color.white;
            _handShowImage.color = activeYellow ? Color.yellow : Color.white;
        }

        private void UpdateShowText(InteractableObject interactableObject)
        {
            _tmpText.text = _localizationService.GetCurrentLanguageStringOrDefault(interactableObject.UIShowTextCode.CurrentValue, Tables.Interactables_name);
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}