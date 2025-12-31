using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Saves
{
    public class JsonFileSerializer
    {
        public enum LoadDeleteStateEnum
        {
            Complete,
            FileWithKeyDontExists,
            Error
        }
        
        public struct SaveCallback
        {
            public bool IsComplete;
            public Exception Exception;
        }
        
        public struct LoadCallback<T>
        {
            public T Value;
            public LoadDeleteStateEnum Callback;
            public Exception Exception;
        }
        
        public struct DeleteCallback
        {
            public LoadDeleteStateEnum Callback;
            public Exception Exception;
        }
        
        public SaveCallback Save(object data, string key, string[] folders = null)
        {
            try
            {
                CreateFolders(folders);
                
                var path = BuildPath(key, folders);
                var json = JsonConvert.SerializeObject(data);

                using var fileStream = new StreamWriter(path);
                fileStream.Write(json);

                return new SaveCallback()
                {
                    IsComplete = true,
                    Exception = null
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveError: {e}");
                return new SaveCallback()
                {
                    IsComplete = false,
                    Exception = e
                };
            }
        }

        private void CreateFolders(string[] folders)
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

        public LoadCallback<T> Load<T>(string key, string[] folders = null)
        {
            try
            {
                var path = BuildPath(key, folders);

                if (File.Exists(path))
                {
                    using var fileStream = new StreamReader(path);
                    var json = fileStream.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<T>(json);
                        
                    return new LoadCallback<T>
                    {
                        Value = data,
                        Callback = LoadDeleteStateEnum.Complete,
                        Exception = null
                    };
                }

                Debug.LogError("LoadError: file not found");
                return new LoadCallback<T>
                {
                    Value = default,
                    Callback = LoadDeleteStateEnum.FileWithKeyDontExists,
                    Exception = null
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadError: {e}");
                return new LoadCallback<T>
                {
                    Value = default,
                    Callback = LoadDeleteStateEnum.Error,
                    Exception = e
                };
            }
        }

        public DeleteCallback Delete(string key, string[] folders = null)
        {
            try
            {
                var path = BuildPath(key, folders);
                
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return new DeleteCallback()
                    {
                        Callback = LoadDeleteStateEnum.Complete,
                        Exception = null
                    };
                }

                Debug.LogError("DeleteError: file not found");
                return new DeleteCallback()
                {
                    Callback = LoadDeleteStateEnum.FileWithKeyDontExists,
                    Exception = null
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteError: {e}");
                return new DeleteCallback()
                {
                    Callback = LoadDeleteStateEnum.Error,
                    Exception = e
                };
            }
        }

        private static string BuildPath(string key, string[] folders = null)
        {
            folders ??= new string[] { };
            var combinedFolders = Path.Combine(folders);
            return Path.Combine(Application.persistentDataPath, combinedFolders, key + ".json");
        }
    }
}