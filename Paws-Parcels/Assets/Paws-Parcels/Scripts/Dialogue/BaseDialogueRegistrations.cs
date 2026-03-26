using Paws_Parcels.Scripts.Saves.States.GameResources;
using Services;
using Yarn.Unity;

namespace NPCDialogs
{
    public class BaseDialogueRegistrations
    {
        private readonly ResourcesService _resourcesService;

        public BaseDialogueRegistrations(DialogueRegistrations dialogueRegistrations, ResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
            dialogueRegistrations.AddCommand("add_money", runner => runner?.AddCommandHandler<int>("add_money", AddMoney));
            dialogueRegistrations.AddCommand("spend_money", runner => runner?.AddCommandHandler<int>("spend_money", SpendMoney));
            dialogueRegistrations.AddFunction("get_money", runner => runner?.AddFunction("get_money", GetMoney));

            //_resourcesService.ObserveResource(ResourcesType.Money).Subscribe(v => Debug.Log(v));
        }

        private void AddMoney(int amount)
        {
            _resourcesService.TryAddResource(ResourcesType.Money, amount);
        }

        private void SpendMoney(int amount)
        {
            _resourcesService.TrySpendResource(ResourcesType.Money, amount);
        }

        private int GetMoney()
        {
            return _resourcesService.GetResourceAmount(ResourcesType.Money);
        }
    }
}