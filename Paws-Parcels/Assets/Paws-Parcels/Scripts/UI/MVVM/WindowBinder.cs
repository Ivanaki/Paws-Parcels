using UnityEngine;

namespace UI.MVVM
{
    public abstract class WindowBinder<T> : MonoBehaviour, IWindowBinder where T : WindowViewModel
    {
        protected T ViewModel;

        public void Bind(WindowViewModel viewModel)
        {
            ViewModel = (T)viewModel;

            OnBind(ViewModel);
        }

        public virtual void Close()
        {
            // Здесь мы сначала будем уничтожать, а потом можно делать анимации на закрытие.
            Destroy(gameObject);
        }

        protected virtual void OnBind(T viewModel) { }
    }
}