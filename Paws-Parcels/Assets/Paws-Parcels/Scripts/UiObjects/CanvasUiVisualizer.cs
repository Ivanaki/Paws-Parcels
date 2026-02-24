using System.Collections.Generic;
using ObservableCollections;

namespace Services
{
    public class CanvasUiVisualizer
    {
        public ObservableHashSet<ShowableInUIBehaviour> ObjectsToShow => _objectsToShow;
        
        private readonly ObservableHashSet<ShowableInUIBehaviour> _objectsToShow = new();
        
        public void ShowOnCanvas(ShowableInUIBehaviour obj)
        {
            _objectsToShow.Add(obj);
        }

        public void HideOnCanvas(ShowableInUIBehaviour obj)
        {
            _objectsToShow.Remove(obj);
        }
    }
}