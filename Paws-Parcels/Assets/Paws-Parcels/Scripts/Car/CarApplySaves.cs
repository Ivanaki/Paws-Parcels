using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarApplySaves : MonoBehaviour
    {
        private GameStateProvider _gameStateProvider;
        private ApplyInFixUpdate _applyInFixUpdate;
        private ReadOnlyReactiveProperty<float> _carSensitivity;
        
        private Rigidbody _rigidbody;
        
        private CompositeDisposable _disposable;
        
        [Inject]
        public void Construct(GameStateProvider gameStateProvider, ApplyInFixUpdate applyInFixUpdate,
            SettingsStateProvider settingsStateProvider)
        {
            _gameStateProvider = gameStateProvider;
            _applyInFixUpdate = applyInFixUpdate;
            _carSensitivity = settingsStateProvider.SettingsProxy.CarSensitivity;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _disposable = new CompositeDisposable();

            //TODO add camera rotation in car
            /*TODO fix bug with save car, машина при загрузке сцены улетает вверх в 1 из сохранений,
            но при загрузке этого сохранения уже в игре, всё норм. Думаю это происходит из за порядка вызова функций,
             и при первом запуске сцены что-то не стыкуется*/
            //_disposable.Add(_carSensitivity.Subscribe(value  =>
            //    _thirdPersonController.SetSensivity(value)));
            
            _disposable.Add(_gameStateProvider.ApplyDataAfterLoad.Subscribe(_ =>
            {
                var carData = _gameStateProvider.GameProxy.CarData;
                
                transform.position = carData.Position;
                transform.rotation = carData.Rotation;
                //_thirdPersonController.SetRotation(playerData.Yaw, playerData.Pitch);
                
                _applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
                {
                    transform.position = carData.Position;
                    transform.rotation = carData.Rotation;
                    _rigidbody.linearVelocity = carData.LinearVelocity;
                    _rigidbody.angularVelocity = carData.AngularVelocity;
                });
            }));

            _disposable.Add(_gameStateProvider.CollectDataBeforeSave.Subscribe(_ =>
            {
                _gameStateProvider.GameProxy.CarData = new CarSaveData
                {
                    Position = transform.position,
                    Rotation = transform.rotation,
                    LinearVelocity = _rigidbody.linearVelocity,
                    AngularVelocity = _rigidbody.angularVelocity
                    //Yaw = _thirdPersonController.GetTargetYaw(),
                    //Pitch = _thirdPersonController.GetTargetPitch()
                };
            }));
        }
        
        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}