using Game.Root;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class CoreTest : MonoBehaviour
    {
        private CoreScenesChanger _coreScenesChanger;
        
        [Inject]
        private void Construct(CoreScenesChanger  coreScenesChanger)
        {
            _coreScenesChanger = coreScenesChanger;
        }
        
        public void MianMenu()
        {
            _coreScenesChanger.GoToMenu();
        }

        public void Restart()
        {
            _coreScenesChanger.Restart();
        }
    }
}