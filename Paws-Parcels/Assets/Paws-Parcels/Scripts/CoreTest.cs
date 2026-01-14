using Game.Root;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class CoreTest : MonoBehaviour
    {
        private CoreSignals _coreSignals;
        
        [Inject]
        private void Construct(CoreSignals  coreSignals)
        {
            _coreSignals = coreSignals;
        }
        
        public void MianMenu()
        {
            //_coreScenesChanger.GoToMenu();
        }

        public void Restart()
        {
            //_coreScenesChanger.Restart();
        }
    }
}