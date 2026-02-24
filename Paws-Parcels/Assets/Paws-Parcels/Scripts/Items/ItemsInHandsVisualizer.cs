using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Items
{
    public class ItemsInHandsVisualizer : MonoBehaviour
    {
        [SerializeField] private Transform _inHandsPosTransform;
        [SerializeField] private Animator _animator;

        private IReadOnlyDictionary<string, ItemInHands> _itemsTypeInHandsPrefabsDict;
        
        private CompositeDisposable _disposables;
        private GameObject _current = null;
        
        private GameProxy _gameProxy { get; set; }

        [Inject]
        private void Construct(ItemsDatabase itemsDatabase, GameStateProvider gameStateProvider)
        {
            _itemsTypeInHandsPrefabsDict = itemsDatabase.ItemsTypeInHandsPrefabsDict;
            _gameProxy = gameStateProvider.GameProxy;
        }

        private void OnEnable()
        {
            _disposables = new CompositeDisposable();
            
            _disposables.Add(_gameProxy.IsItemInHandsNull.Subscribe(value =>
            {
                if (value)
                {
                    DestroyCurrent();
                    _animator.SetLayerWeight(1, 0f);
                }
                else
                {
                    CreateNew();
                    _animator.SetLayerWeight(1, 1f);
                }
            }));
            
            _disposables.Add(_gameProxy.InHandsItemTypeId.Subscribe(newType =>
            {
                DestroyCurrent();
                CreateNew();
            }));
        }

        private void DestroyCurrent()
        {
            if(_current != null)
                Destroy(_current);
            _current = null;
        }

        private void CreateNew()
        {
            var itemTypeId = _gameProxy.InHandsItemTypeId.Value;
                    
            if (string.IsNullOrEmpty(itemTypeId) == false)
            {
                if (_itemsTypeInHandsPrefabsDict.TryGetValue(itemTypeId, out var itemInHands))
                {
                    var t = Instantiate(itemInHands, _inHandsPosTransform);
                    t.transform.localPosition = t.LocalPosition;
                    t.transform.localRotation = t.LocalRotation;
                    _current = t.gameObject;
                }
            }
            
            //_animator.SetLayerWeight(1, 1f);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
        }
    }
}