using Car_Player;
using Quests;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class HintUIObject : UiObject
    {
        [SerializeField] private Image _image;
        
        private CompositeDisposable _disposable;
        
        public override void Construct(IShowableInUI showableInUIObject)
        {
            var hint = showableInUIObject as Hint;
            
            if (hint == null)
            {
                Debug.LogWarning("HintUIObject has no Hint");
                return;
            }

            _disposable = new CompositeDisposable();
            
            _disposable.Add(hint.Image.Subscribe(v =>
            {
                if(v != null)
                    _image.sprite = v;
            }));
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}