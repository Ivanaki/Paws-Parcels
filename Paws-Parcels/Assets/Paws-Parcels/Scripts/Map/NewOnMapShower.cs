using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;

namespace Map
{
    public class NewOnMapShower : MonoBehaviour
    {
        public List<MapUi> mapUis = new(); 
        
        private readonly CompositeDisposable _disposable = new();

        [Inject] private BuildingsLogic _buildingsLogic;
        
        private void Awake()
        {
            mapUis.ForEach(x => x.SetMarked(false));
            
            _disposable.Add(_buildingsLogic.Selected.Subscribe(id =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    mapUis.ForEach(x => x.SetMarked(false));
                    return;
                }
                mapUis.FirstOrDefault(x => x.MapId == id)?.SetMarked(true);
            }));
            _disposable.Add(_buildingsLogic.Deselected.Subscribe(_ =>
            {
                mapUis.ForEach(x => x.SetMarked(false));
            }));
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}