using System;
using R3;
using TMPro;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class MoneyVisualizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tmpText;
        
        private MoneySystem _moneySystem;
        private IDisposable _disposable;
        
        [Inject]
        private void Construct(MoneySystem moneySystem)
        {
            _moneySystem = moneySystem;
        }

        private void OnEnable()
        {
            _disposable = _moneySystem.Money.Subscribe(value =>
            {
                _tmpText.text = value.ToString();
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}