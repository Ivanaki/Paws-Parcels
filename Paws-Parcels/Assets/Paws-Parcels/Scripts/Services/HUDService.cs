
using Paws_Parcels.Scripts.Saves.States.GameResources;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;

namespace UI.HUD
{
    public class HUDService
    {
        public ReadOnlyReactiveProperty<int> Money { get; }
        public ReadOnlyReactiveProperty<bool> IsCompassActive => _isCompassActive;

        private readonly ReactiveProperty<bool> _isCompassActive;

        //сюда интерфейсы или readOnly по примеру и cmd, но я отказываюсь от CommandProcessor,
        //поэтому кидаю просто коллекции без инкапсуляции
        public HUDService(GameStateProvider provider, ResourcesService resourcesService)
        {
            _isCompassActive = provider.GameProxy.IsCompassActive;
            Money = resourcesService.ObserveResource(ResourcesType.Money);
        }
        
        public void ShowCompas()
        {
            _isCompassActive.Value = true;
        }
        
        public void HideCompas()
        {
            _isCompassActive.Value = false;
        }
    }
}