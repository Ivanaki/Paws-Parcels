using UnityEngine;

namespace SaveLaod
{
    public class ScreenShotCallback
    {
        public bool IsLoaded;
        public Texture2D Texture;
        
        public ScreenShotCallback(bool isLoaded, Texture2D texture)
        {
            IsLoaded = isLoaded;
            Texture = texture;
        }
    }
}