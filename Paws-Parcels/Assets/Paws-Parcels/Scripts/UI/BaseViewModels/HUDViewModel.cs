using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;

namespace UI.HUD
{
    public class HUDViewModel
    {
        //public ObservableHashSet<ShowableInUIBehaviour> ObjectsToShow { get; }
        //public IReadOnlyDictionary<string, UiObject> ObjectsPrefabsDict { get; }
        public ReadOnlyReactiveProperty<bool> IsCompassActive { get; }
        public ReadOnlyReactiveProperty<bool> ActiveHud { get; }
        
        public ReadOnlyReactiveProperty<int> Money { get; }
        
        public GameProxy GameProxy { get; }
        
        public HUDViewModel(HUDService hudService, ShowableInterfaceService showableInterfaceService, GameProxy gameProxy)
        {
            //ObjectsToShow = objectsToShow;
            //ObjectsPrefabsDict = objectsPrefabsDict;
            ActiveHud = showableInterfaceService.HudActive;
            IsCompassActive = hudService.IsCompassActive;
            Money = hudService.Money;
            GameProxy = gameProxy;
        }
    }
}