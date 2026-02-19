using System;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class MaxPlayerRangeNearCar : MonoBehaviour
    {
        [SerializeField] private PlayerInCollider _playerInCollider;
        //[SerializeField, Range(0f, 1f)] private float _maxPlayerRangeNearCarCoof = 0.95f;
        [SerializeField] private PlayerInCollider[] _allowedColliders;

        
        
        private CompositeDisposable _disposable;
        private PlayerSpawn _playerSpawn;
        private SoloMonolog _soloMonolog;
        private LanguageChanger _languageChanger;
        private Transform _playerTransform;
        private PlayerLastPositionRememberer _playerLastPositionRememberer;

        [Inject]
        private void Construct(PlayerSpawn playerSpawn, SoloMonolog soloMonolog, LanguageChanger languageChanger,
            Player player, PlayerLastPositionRememberer playerLastPositionRememberer)
        {
            _playerSpawn = playerSpawn;
            _soloMonolog = soloMonolog;
            _languageChanger = languageChanger;
            _playerTransform = player.transform;
            _playerLastPositionRememberer = playerLastPositionRememberer;
        }
        
        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;
            
            
            
            foreach (var playerInCollider in _allowedColliders)
            {
                dis = playerInCollider.IsPlayerInCollider.Skip(1).Subscribe(value =>
                {
                    if (!value)
                    {
                        var res = TryAllowedColliders(playerInCollider);
                        if(res) return;

                        if (_playerInCollider.IsPlayerInCollider.Value) return;
                    
                        SetPlayerPosition(playerInCollider);
                    }
                }); _disposable.Add(dis);
            }
            
            dis = _playerInCollider.IsPlayerInCollider.Skip(1).Subscribe(value =>
            {
                if (!value)
                {
                    var res = TryAllowedColliders(null);
                    if(res) return;

                    SetPlayerPosition(_playerInCollider);
                }
            }); _disposable.Add(dis);
        }

        private bool TryAllowedColliders(PlayerInCollider playerInCollider)
        {
            return _allowedColliders.Where(x => x != playerInCollider).
                Any(x => x.IsPlayerInCollider.Value);
        }
        
        private void OnDisable()
        {
            _disposable?.Dispose();
        }
        
        private void SetPlayerPosition(PlayerInCollider playerInCollider)
        {
            //var vector = _playerTransform.position - playerInCollider.transform.position;
            //var newPlayerPosition = playerInCollider.transform.position + vector * _maxPlayerRangeNearCarCoof;
            _playerSpawn.Spawn(_playerLastPositionRememberer.GetLastPosition());
            var phrases = _languageChanger.GetDialog(DialogEnum.PlayerIsFarFromCar);
            _soloMonolog.StartMonolog(phrases[0].Phrases);
        }
    }
}