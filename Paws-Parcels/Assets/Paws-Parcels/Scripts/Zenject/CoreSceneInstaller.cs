using Game.Root;
using Paws_Parcels.Scripts.Saves.States;
using UI.Core;
using UI.HUD;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts.Zenject
{
    public class CoreSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<CoreScenesChanger>().AsSingle();
            Container.Bind<HUDService>().AsSingle();
            
            
            
            //UIViewModels: (по идее должно быть в отдельном контейнере, но в зенжекте такого нет)
            Container.Bind<CoreUIManager>().AsSingle();
            Container.Bind<UICoreRootViewModel>().AsSingle();
        }
    }
}