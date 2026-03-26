using System;
using R3;

namespace Extentions
{
    public static class ObservableExtensions
    {
        public static void OnNextAndDispose<T>(this ISubject<T> observer, T value)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            observer.OnNext(value);

            if (observer is IDisposable disposable)
                disposable.Dispose();
        }
    }
}