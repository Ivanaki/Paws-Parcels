using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class DoorsVisualizer : MonoBehaviour
    {
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int OpenInstance = Animator.StringToHash("OpenInstance");
        private static readonly int CloseInstance = Animator.StringToHash("CloseInstance");
        
        [SerializeField] private Animator _doorsAnimator;
        [SerializeField] private GameObject _stairsActivatePanel;
        [SerializeField] private GameObject _blockActivatePanel;
        
        private ReadOnlyReactiveProperty<bool> _isCarDoorOpen;
        private CompositeDisposable _disposable;
        private GameStateProvider _gameStateProvider;
        
        [Inject]
        private void Construct(GameStateProvider gameStateProvider)
        {
            _isCarDoorOpen = gameStateProvider.GameProxy.IsCarDoorOpen;
            _gameStateProvider = gameStateProvider;
        }

        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            
            _disposable.Add(_isCarDoorOpen.Subscribe(value =>
            {
                RefreshDoors(value, false);
            }));
            
            _disposable.Add(_gameStateProvider.ApplyDataAfterLoad.Subscribe(_ =>
            {
                RefreshDoors(_isCarDoorOpen.CurrentValue, true);
            }));
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
        
        private void RefreshDoors(bool isOpen, bool isInstance)
        {
            if (isInstance)
                _doorsAnimator.SetTrigger(isOpen ?  OpenInstance : CloseInstance);
            else
                _doorsAnimator.SetTrigger(isOpen ?  Open : Close);
            
            _stairsActivatePanel.SetActive(isOpen);
            _blockActivatePanel.SetActive(!isOpen);
        }
    }
}