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
        
        public HUDViewModel(HUDService hudService, ShowableInterfaceService showableInterfaceService/*,
            ObservableHashSet<ShowableInUIBehaviour> objectsToShow,
            IReadOnlyDictionary<string, UiObject> objectsPrefabsDict */)
        {
            //ObjectsToShow = objectsToShow;
            //ObjectsPrefabsDict = objectsPrefabsDict;
            ActiveHud = showableInterfaceService.HudActive;
            IsCompassActive = hudService.IsCompassActive;
        }
    }
}