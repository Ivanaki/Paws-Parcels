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
        
        public static float VectorToGlobalYRotation(Vector3 direction)
        {
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return 0f;

            direction.Normalize();
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            return angle;
        }
    }
}