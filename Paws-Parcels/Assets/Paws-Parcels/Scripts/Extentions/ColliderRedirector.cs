using NaughtyAttributes;
using UnityEngine;

namespace Paws_Parcels.Extentions
{
    public class ColliderRedirector : MonoBehaviour
    {
        [SerializeField] private bool _notParent = false;
        [ShowIf(nameof(_notParent))]
        [SerializeField] private GameObject _mainObject;
        
        /// <summary>
        /// если мы знаем, что этот объект может быть найден другим скриптом с помощью коллайдера
        /// </summary>
        /// <param name="mainScript"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryGetComponentBase<T>(out T mainScript)
        {
            if(_notParent)
                return _mainObject.TryGetComponent(out mainScript);
            else
                return transform.parent.TryGetComponent(out mainScript);
        }

        
    }
}