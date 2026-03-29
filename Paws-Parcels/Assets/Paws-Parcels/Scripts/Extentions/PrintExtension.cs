using UnityEngine;

namespace Extentions
{
    public static class PrintExtension
    {
        public static string GetFullPath(this Transform t)
        {
            string path = t.name;

            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }

            return path;
        }
    }
}