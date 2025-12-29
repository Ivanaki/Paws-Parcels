using System;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class MoneySystem : IInitializable, IDisposable
    {
        private readonly GameStateProvider _gameStateProvider;
        
        private IDisposable _disposable;
        
        public MoneySystem(GameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }
        
        public ReadOnlyReactiveProperty<int> Money => _money;
        private ReactiveProperty<int> _money = new(0);

        public void AddMoney(int amount)
        {
            amount = Mathf.Clamp(amount, 0, int.MaxValue);
            _money.Value += amount;
        }
        
        public void SpendMoney(int amount)
        {
            amount = Mathf.Clamp(amount, 0, int.MaxValue);
            _money.Value -= amount;
        }

        public void Initialize()
        {
            _disposable = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.Money = _money.Value;
            }); 
            
            var dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                _money.Value = _gameStateProvider.CoreState.Money;
            }); _disposable = Disposable.Combine(_disposable, dis);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}