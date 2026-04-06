using Car_Player;
using Quests;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class HintUIObject : UiObject
    {
        [SerializeField] private Transform _grid;
        
        private CompositeDisposable _disposables;
        private GameObject _currentHint;
        
        public override void Construct(IShowableInUI showableInUIObject)
        {
            var hint = showableInUIObject as Hint;
            
            if (hint == null)
            {
                Debug.LogWarning("HintUIObject has no Hint");
                return;
            }

            _disposables = new CompositeDisposable();
            
            _disposables.Add(hint.Prefab.Subscribe(prefab =>
            {
                if(prefab == null) return;
                
                if(_currentHint != null) 
                    Destroy(_currentHint);
                
                _currentHint = Instantiate(prefab, _grid);
                _currentHint.transform.localPosition = Vector3.zero;
                _currentHint.transform.localRotation = Quaternion.identity;
            }));
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}