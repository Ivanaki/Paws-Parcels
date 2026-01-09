using Game.Root;
using UI.MainMenu;
using Zenject;

namespace Paws_Parcels.Scripts.Zenject
{
    public class MainMenuSceneInstaller : MonoInstaller
    {        
        
        
        public override void InstallBindings()
        {
            Container.Bind<MainMenuSignals>().AsSingle();
            
            
            //UIViewModels: (по идее должно быть в отдельном контейнере, но в зенжекте такого нет)
            Container.Bind<MainMenuUIManager>().AsSingle();
            Container.Bind<UIMainMenuRootViewModel>().AsSingle();
        }
    }
}