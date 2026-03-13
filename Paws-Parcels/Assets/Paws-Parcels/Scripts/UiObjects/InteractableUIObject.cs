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
        [SerializeField] private Image _image;
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

            _disposable.Add(_localizationService.CurrentLocale.Subscribe(locale =>
            {
                _tmpText.text = _localizationService.GetCurrentLanguageStringOrDefault(interactableObject.UIShowTextCode, Tables.Interactables_name);
            }));
            
            _disposable.Add(interactableObject.IsQuestSelect.Subscribe(v =>
            {
                _image.color = v ? Color.yellow : Color.white;
                _handShowImage.color = v ? Color.yellow : Color.white;
            }));

            _disposable.Add(interactableObject.CanInteract.Subscribe(v =>
            {
                _handShowImage.gameObject.SetActive(v);
                _tmpText.gameObject.SetActive(v);
            }));
            
            _disposable.Add(interactableObject.IsQuestActive.Subscribe(v =>
            {
                RectTransform.sizeDelta = v ? new Vector2(68, 68) : new Vector2(25, 25);
            }));
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}