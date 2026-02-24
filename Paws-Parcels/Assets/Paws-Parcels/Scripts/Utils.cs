using UnityEngine;

namespace Paws_Parcels.Scripts
{
    public static class Utils
    {
        public static Texture2D CreateWhiteTexture()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return tex;
        }
    }
}