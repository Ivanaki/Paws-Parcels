using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Paws_Parcels.Scripts.Saves
{
    public static class SaveStatic
    {
        public static string BuildFilePath(string key, string extension, string[] folders = null)
        {
            var buildedFolders = BuildFoldersPath(folders);
            return Path.Combine(buildedFolders, key + extension);
        }

        public static string BuildFoldersPath(string[] folders)
        {
            folders ??= new string[] { };
            var combinedFolders = Path.Combine(folders);
            return Path.Combine(Application.persistentDataPath, combinedFolders);
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
        
        public static int GetNextIndexOfFoldersWithSamePrefix(string folder, string prefix)
        {
            var path = Path.Combine(Application.persistentDataPath, folder);
            
            if (!Directory.Exists(path))
            {
                Debug.LogWarning("Папка не существует: " + path);
                return 1;
            }
            
            var maxIndex = 0;

            // Паттерн: AutoSave + число в конце
            var regex = new Regex($"^{prefix}(\\d+)$");
            
            var directories = Directory.GetDirectories(path);
            
            foreach (var dir in directories)
            {
                var match = regex.Match(dir);

                if (match.Success)
                {
                    var value = int.Parse(match.Groups[1].Value);
                    if (value > maxIndex)
                        maxIndex = value;
                }
            }

            return maxIndex + 1;
        }

        public static string[] GetFoldersNameInFolder(string[] foldersPath)
        {
            CreateFolders(foldersPath);
            var path = Path.Combine(Application.persistentDataPath, Path.Combine(foldersPath));
            var folderWithPath = Directory.GetDirectories(path);
            return folderWithPath.Select(Path.GetFileName).ToArray();
        }

        public static string[] GetFoldersNameInFolder(string folderPath)
        {
            return GetFoldersNameInFolder(new []{folderPath});
        }
    }
}