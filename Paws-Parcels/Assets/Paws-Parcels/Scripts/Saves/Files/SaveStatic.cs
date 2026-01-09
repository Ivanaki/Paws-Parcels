using System.IO;
using UnityEngine;

namespace Paws_Parcels.Scripts.Saves
{
    public static class SaveStatic
    {
        public static string BuildPath(string key, string extension, string[] folders = null)
        {
            folders ??= new string[] { };
            var combinedFolders = Path.Combine(folders);
            return Path.Combine(Application.persistentDataPath, combinedFolders, key + extension);
        }
        
        public static void CreateFolders(string[] folders)
        {
            if(folders == null) return;

            var path = Application.persistentDataPath;
            foreach (var folder in folders)
            {
                path = Path.Combine(path, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
        
        /*public static bool CheckFileExists(string key, string extension, string[] folders = null)
        {
            if (File.Exists(BuildPath(key, extension, folders)))
            {
                return true;
            }
            return false;
        }*/
    }
}