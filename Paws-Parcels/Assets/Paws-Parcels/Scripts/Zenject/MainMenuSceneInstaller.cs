using Game.Root;
using Zenject;

namespace Paws_Parcels.Scripts.Zenject
{
    public class MainMenuSceneInstaller : MonoInstaller
    {        
        
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainMenuEntryPoint>().AsSingle().NonLazy();
            Container.BindExecutionOrder<MainMenuEntryPoint>(-1);
            Container.Bind<MainMenuScenesChanger>().AsSingle();
        }
    }
}