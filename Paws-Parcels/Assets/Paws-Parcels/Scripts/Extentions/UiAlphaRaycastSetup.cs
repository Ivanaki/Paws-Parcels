using System;
using UnityEngine;
using UnityEngine.UI;

namespace Extentions
{
    [RequireComponent(typeof(Image))]
    public class UiAlphaRaycastSetup : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}