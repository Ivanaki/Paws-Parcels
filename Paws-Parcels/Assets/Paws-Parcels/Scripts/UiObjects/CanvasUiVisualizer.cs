using System.Collections.Generic;
using ObservableCollections;

namespace Services
{
    public class CanvasUiVisualizer
    {
        public ObservableHashSet<IShowableInUI> ObjectsToShow => _objectsToShow;
        
        private readonly ObservableHashSet<IShowableInUI> _objectsToShow = new();
        
        public void ShowOnCanvas(IShowableInUI obj)
        {
            _objectsToShow.Add(obj);
        }

        public void HideOnCanvas(IShowableInUI obj)
        {
            _objectsToShow.Remove(obj);
        }
    }
}