using Game.Root;
using Zenject;

namespace MotorHome.Scripts.Zenject
{
    public class CoreSceneInstaller : MonoInstaller
    {
       
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CoreEntryPoint>().AsSingle().NonLazy();
            Container.BindExecutionOrder<CoreEntryPoint>(-1);
            Container.Bind<CoreScenesChanger>().AsSingle();
        }
    }
}