using Game.Root;
using Services;
using Services.Music;
using UI.MainMenu;
using Zenject;

namespace Paws_Parcels.Scripts.Installers
{
    public class MainMenuSceneInstaller : MonoInstaller
    {        
        
        
        public override void InstallBindings()
        {
            Container.Bind<MainMenuSignals>().AsSingle();
            
            
            //UIViewModels: (по идее должно быть в отдельном контейнере, но в зенжекте такого нет)
            Container.Bind<MainMenuUIManager>().AsSingle();
            Container.Bind<UIMainMenuRootViewModel>().AsSingle();
            
            //инпут с клавиатуры и мышки, можно считать это тоже ViewModelsContainer
            Container.BindInterfacesTo<MainMenuBackService>().AsSingle();
            Container.BindInterfacesTo<MainMenuMusicService>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<InputsMainMenuService>().AsSingle().NonLazy();
        }
    }
}
