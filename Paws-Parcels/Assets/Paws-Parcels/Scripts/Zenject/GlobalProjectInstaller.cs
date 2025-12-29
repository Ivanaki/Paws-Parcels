using Game.Root;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts.Zenject
{
    public class GlobalProjectInstaller : MonoInstaller
    {
        [SerializeField] private UIRootView _uiRootPrefab;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameEntryPoint>().AsSingle().NonLazy();
            Container.BindExecutionOrder<GameEntryPoint>(-1000);

            Container.Bind<UIRootView>().FromComponentInNewPrefab(_uiRootPrefab).AsSingle()
                .OnInstantiated<UIRootView>((ctx, ui) =>
                {
                    ui.ShowLoadingScreen();
                    ui.transform.SetParent(null);
                    DontDestroyOnLoad(ui.gameObject);
                }).NonLazy();
            
            Container.Bind<Coroutines>().FromNewComponentOnNewGameObject().WithGameObjectName("[COROUTINES]").AsSingle()
                .OnInstantiated<Coroutines>((ctx, c) =>
                {
                    c.transform.SetParent(null);
                    DontDestroyOnLoad(c.gameObject);
                }).NonLazy();
        }
    }
}