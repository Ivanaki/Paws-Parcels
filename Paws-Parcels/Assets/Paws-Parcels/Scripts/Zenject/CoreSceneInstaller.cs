using Game.Root;
using Paws_Parcels.Scripts.Player;
using Services;
using UI.Core;
using UI.HUD;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts.Installers
{
    public class CoreSceneInstaller : MonoInstaller
    {
        //[SerializeField] private PlayerEmpty _playerEmpty;
        
        public override void InstallBindings()
        {
            //Container.Bind<PlayerEmpty>().FromInstance(_playerEmpty).AsSingle();
            
            Container.Bind<CoreSignals>().AsSingle();
            Container.Bind<HUDService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<InteractableService>().AsSingle();
            
            //UIViewModels: (по идее должно быть в отдельном контейнере, но в зенжекте такого нет)
            Container.Bind<CoreUIManager>().AsSingle();
            Container.Bind<UICoreRootViewModel>().AsSingle();
            
            //инпут с клавиатуры и мышки, можно считать это тоже ViewModelsContainer
            Container.BindInterfacesTo<CoreBackService>().AsSingle();
        }
    }
}