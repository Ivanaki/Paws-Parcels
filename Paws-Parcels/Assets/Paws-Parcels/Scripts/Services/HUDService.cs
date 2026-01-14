
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;

namespace UI.HUD
{
    public class HUDService
    {
        public ReadOnlyReactiveProperty<bool> IsCompassActive => _isCompassActive;

        private readonly ReactiveProperty<bool> _isCompassActive;

        //сюда интерфейсы или readOnly по примеру и cmd, но я отказываюсь от CommandProcessor,
        //поэтому кидаю просто коллекции без инкапсуляции
        public HUDService(GameStateProvider provider)
        {
            _isCompassActive = provider.GameProxy.IsCompassActive;
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