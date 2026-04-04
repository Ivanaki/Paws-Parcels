using Paws_Parcels.Extentions;
using UnityEngine;

namespace Extentions
{
    public static class GetComponentMy
    {
        public static T Get<T>(this Component gameObjectToTry) where T : MonoBehaviour
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

        public static bool TryGetComponentMy<T>(this Component gameObjectToTry, out T mainScript) where T : MonoBehaviour
        {
            mainScript = Get<T>(gameObjectToTry);
            return mainScript != null;
        }
    }
}