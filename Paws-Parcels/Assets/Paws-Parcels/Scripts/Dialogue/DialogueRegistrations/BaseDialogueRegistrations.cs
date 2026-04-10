using System.Linq;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using Services;
using UI.DialogueScreen;
using Yarn.Unity;

namespace NPCDialogs
{
    public class BaseDialogueRegistrations
    {
        private readonly ResourcesService _resourcesService;

        public BaseDialogueRegistrations(DialogueRegistrations dialogueRegistrations, ResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
            dialogueRegistrations.RegisterCommand("add_money", runner => runner?.AddCommandHandler<int>("add_money", AddMoney));
            dialogueRegistrations.RegisterCommand("spend_money", runner => runner?.AddCommandHandler<int>("spend_money", SpendMoney));
            dialogueRegistrations.RegisterFunction("get_money", runner => runner?.AddFunction("get_money", GetMoney));
            dialogueRegistrations.RegisterCommand("set_emotion", runner =>
            {
                if (runner == null)
                {
                    return;
                }

                void HandleSetEmotion(string emotionOrTarget, string emotion = null)
                {
                    var presenter = ResolvePresenter(runner);
                    if (presenter == null)
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(emotion))
                    {
                        presenter.SetEmotion(emotionOrTarget);
                        return;
                    }

                    presenter.SetEmotion(emotionOrTarget, emotion);
                }

                runner.AddCommandHandler<string, string>("set_emotion", HandleSetEmotion);
            });
            dialogueRegistrations.RegisterCommand("set_player_emotion", runner => runner?.AddCommandHandler<string>("set_player_emotion", emotion =>
            {
                ResolvePresenter(runner)?.SetPlayerEmotion(emotion);
            }));
            dialogueRegistrations.RegisterCommand("set_npc_emotion", runner => runner?.AddCommandHandler<string>("set_npc_emotion", emotion =>
            {
                ResolvePresenter(runner)?.SetNpcEmotion(emotion);
            }));

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

        private static CustomLinePresenter ResolvePresenter(DialogueRunner runner)
        {
            return runner?.DialoguePresenters
                .OfType<CustomLinePresenter>()
                .FirstOrDefault();
        }
    }
}
