using System;
using System.IO;
using Game.Root;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class MainTest : MonoBehaviour
    {
        private MainMenuSignals _mainMenuSignals;
        
        [Inject]
        private void Construct(MainMenuSignals  mainMenuSignals)
        {
            _mainMenuSignals = mainMenuSignals;
        }
        
        public void Core()
        {
            //_mainMenuScenesChanger.GoToCore();
        }

        public void Restart()
        {
            //_mainMenuScenesChanger.Restart();
        }

        public void Quit()
        {
            //_mainMenuScenesChanger.Exit();
        }

        private void Start()
        {
            print(Path.Combine(new string[]{}));
        }
    }
}