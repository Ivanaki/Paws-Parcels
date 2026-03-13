#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Paws_Parcels.Extentions
{
    public static class SavesDeleter
    {
        [MenuItem("Tools/Clear Saves Folder")]
        private static void ClearSavesFolder()
        {
            string savesPath = Path.Combine(Application.persistentDataPath, "Saves");
    
            if (Directory.Exists(savesPath))
            {
                Directory.Delete(savesPath, true);
                Debug.Log($"Папка Saves успешно удалена по пути: {savesPath}");
            }
            else
            {
                Debug.LogWarning($"Папка Saves не найдена по пути: {savesPath}");
            }
        }
    }
}
#endif