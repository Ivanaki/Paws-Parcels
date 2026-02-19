using System.Collections.Generic;
using ObservableCollections;
using R3;
using Services;

namespace UI.HUD
{
    public class HUDViewModel
    {
        public ObservableHashSet<IShowableInUIObject> ObjectsToShow { get; }
        public IReadOnlyDictionary<string, UiObject> ObjectsPrefabsDict { get; }
        public ReadOnlyReactiveProperty<bool> IsCompassActive { get; }
        
        public HUDViewModel(HUDService hudService, 
            ObservableHashSet<IShowableInUIObject> objectsToShow,
            IReadOnlyDictionary<string, UiObject> objectsPrefabsDict )
        {
            ObjectsToShow = objectsToShow;
            ObjectsPrefabsDict = objectsPrefabsDict;
            IsCompassActive = hudService.IsCompassActive;
        }
    }
}