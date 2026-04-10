using System.Linq;
using NaughtyAttributes;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class DiaryInteractable : BaseInteractable
    {
        private enum RewardMode
        {
            OpenDiaryPage = 0,
            GiveMoney = 1
        }

        [Header("Reward")]
        [SerializeField] private RewardMode _rewardMode = RewardMode.OpenDiaryPage;
        [SerializeField, ShowIf(nameof(IsDiaryPageReward)), AllowNesting] private string _diaryName = "first";
        [SerializeField, ShowIf(nameof(IsMoneyReward)), AllowNesting, Min(0)] private int _moneyReward = 0;
        [SerializeField] private GameObject _gameObject;

        [Inject] private GameProxy _gameProxy;

        private bool IsDiaryPageReward => _rewardMode == RewardMode.OpenDiaryPage;
        private bool IsMoneyReward => _rewardMode == RewardMode.GiveMoney;

        public override void Interact()
        {
            switch (_rewardMode)
            {
                case RewardMode.GiveMoney:
                    AddMoneyReward();
                    break;
                case RewardMode.OpenDiaryPage:
                default:
                    AddDiaryPage();
                    break;
            }

            _gameObject?.SetActive(false);
            gameObject.SetActive(false);
        }

        private void AddDiaryPage()
        {
            if (string.IsNullOrWhiteSpace(_diaryName))
            {
                return;
            }

            if (_gameProxy.DiaryPages.Contains(_diaryName) == false)
            {
                _gameProxy.DiaryPages.Add(_diaryName);
            }
        }

        private void AddMoneyReward()
        {
            if (_moneyReward <= 0)
            {
                return;
            }

            var moneyResource = _gameProxy.Resources.FirstOrDefault(resource => resource.ResourcesType == ResourcesType.Money);

            if (moneyResource == null)
            {
                moneyResource = new ResourceProxy(new ResourceData
                {
                    ResourcesType = ResourcesType.Money,
                    Amount = 0
                });

                _gameProxy.Resources.Add(moneyResource);
            }

            moneyResource.Amount.Value += _moneyReward;
        }
    }
}
