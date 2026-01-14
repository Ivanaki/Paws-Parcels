using R3;

namespace UI.HUD
{
    public class HUDViewModel
    {
        public readonly ReadOnlyReactiveProperty<bool> IsCompassActive;
        
        public HUDViewModel(HUDService hudService)
        {
           IsCompassActive = hudService.IsCompassActive;
        }
    }
}