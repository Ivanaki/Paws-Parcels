using System.Collections.Generic;
using ObservableCollections;

namespace Services
{
    public class CanvasUiVisualizer
    {
        public ObservableHashSet<IShowableInUIObject> ObjectsToShow => _objectsToShow;
        
        private readonly ObservableHashSet<IShowableInUIObject> _objectsToShow = new();
        
        public void ShowOnCanvas(IShowableInUIObject obj)
        {
            _objectsToShow.Add(obj);
        }

        public void HideOnCanvas(IShowableInUIObject obj)
        {
            _objectsToShow.Remove(obj);
        }
    }
}