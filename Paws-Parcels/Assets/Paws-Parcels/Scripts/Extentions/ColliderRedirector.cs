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

        public static T Get<T>(GameObject gameObjectToTry) where T : MonoBehaviour
        {
            var interactableObject = gameObjectToTry.GetComponent<T>();
            if (interactableObject == null)
            {
                if (gameObjectToTry.TryGetComponent(out ColliderRedirector colliderRedirector))
                {
                    colliderRedirector.TryGetComponentBase(out interactableObject);
                }
            }

            return interactableObject;
        }

        public static bool TryGet<T>(GameObject gameObjectToTry, out T mainScript) where T : MonoBehaviour
        {
            mainScript = Get<T>(gameObjectToTry);
            return mainScript != null;
        }
    }
}