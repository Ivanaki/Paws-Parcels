using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI.MVVM
{
    public class WindowsContainer : MonoBehaviour
    {
        [SerializeField] private Transform _screensContainer;
        [SerializeField] private Transform _popupsContainer;
        
        private readonly Dictionary<WindowViewModel, IWindowBinder> _openedPopupBinders = new();
        private IWindowBinder _openedScreenBinder;

        [Inject] private DiContainer _container;
        
        public void OpenPopup(WindowViewModel viewModel)
        {
            var prefabPath = GetPopupPrefabPath(viewModel);
            var prefab = Resources.Load<GameObject>(prefabPath);
            var createdPopup = _container.InstantiatePrefab(prefab, _popupsContainer);
            var binder = createdPopup.GetComponent<IWindowBinder>();
            
            binder.Bind(viewModel);
            _openedPopupBinders.Add(viewModel, binder);
        }

        public void ClosePopup(WindowViewModel popupViewModel)
        {
            var binder = _openedPopupBinders[popupViewModel];
            
            binder?.Close();
            _openedPopupBinders.Remove(popupViewModel);
        }

        public void OpenScreen(WindowViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }
            
            _openedScreenBinder?.Close();
            
            var prefabPath = GetScreenPrefabPath(viewModel);
            var prefab = Resources.Load<GameObject>(prefabPath);
            var createdScreen = _container.InstantiatePrefab(prefab, _screensContainer);
            var binder = createdScreen.GetComponent<IWindowBinder>();
            
            binder.Bind(viewModel);
            _openedScreenBinder = binder;
        }

        public void CloseScreen()
        {
            if (_openedScreenBinder != null)
            {
                _openedScreenBinder.Close();
                _openedScreenBinder = null;
            }
        }
        
        private static string GetScreenPrefabPath(WindowViewModel viewModel)
        {
            return $"UI/Screens/{viewModel.Id}";
        }
        
        private static string GetPopupPrefabPath(WindowViewModel viewModel)
        {
            return $"UI/Popups/{viewModel.Id}";
        }
    }
}