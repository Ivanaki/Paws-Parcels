using System;
using System.IO;
using Newtonsoft.Json;
using Paws_Parcels.Scripts.Saves;
using UnityEngine;

namespace Saves
{
    public class JsonFileSerializer
    {
        private const string Extension = ".json";
        
        public struct SaveCallback
        {
            public bool IsComplete;
            public Exception Exception;
        }
        
        public struct LoadCallback<T>
        {
            public T Value;
            public FileOperationResult Callback;
            public Exception Exception;
        }
        
        public struct DeleteCallback
        {
            public FileOperationResult Callback;
            public Exception Exception;
        }

        public JsonFileSerializer()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
        }
        
        public SaveCallback Save<T>(T data, string key, string[] folders = null)
        {
            try
            {
                SaveStatic.CreateFolders(folders);

                var path = SaveStatic.BuildFilePath(key, Extension, folders);
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);

                using var fileStream = new StreamWriter(path);
                fileStream.Write(json);

                return new SaveCallback()
                {
                    IsComplete = true,
                    Exception = new()
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveError: {e.Message}");
                return new SaveCallback()
                {
                    IsComplete = false,
                    Exception = e
                };
            }
        }

        public SaveCallback Save<T>(T data, string key, string folder)
        {
            return Save(data, key, new[] { folder });
        }

        public SaveCallback Save<T>(T data, string key, string folder1, string folder2)
        {
            return Save(data, key, new[] { folder1, folder2 });
        }


        public LoadCallback<T> Load<T>(string key, string[] folders = null)
        {
            try
            {
                var path = SaveStatic.BuildFilePath(key, Extension, folders);

                if (File.Exists(path))
                {
                    using var fileStream = new StreamReader(path);
                    var json = fileStream.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<T>(json);

                    if (data == null)
                    {
                        return new LoadCallback<T>
                        {
                            Value = default,
                            Callback = FileOperationResult.Error,
                            Exception = new Exception("LoadError: file is broken")
                        };
                    }
                        
                    return new LoadCallback<T>
                    {
                        Value = data,
                        Callback = FileOperationResult.Success,
                        Exception = new ()
                    };
                }

                Debug.LogError("LoadError: file not found");
                return new LoadCallback<T>
                {
                    Value = default,
                    Callback = FileOperationResult.NotFound,
                    Exception = new()
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadError: {e.Message}");
                //Debug.Log("test");
                return new LoadCallback<T>
                {
                    Value = default,
                    Callback = FileOperationResult.Error,
                    Exception = e
                };
            }
        }

        public LoadCallback<T> Load<T>(string key, string folder)
        {
            return Load<T>(key, new[] { folder });
        }

        public LoadCallback<T> Load<T>(string key, string folder1, string folder2 )
        {
            return Load<T>(key, new[] { folder1, folder2 });
        }


        public DeleteCallback Delete(string key, string[] folders = null)
        {
            try
            {
                var path = SaveStatic.BuildFilePath(key, Extension, folders);
                
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return new DeleteCallback()
                    {
                        Callback = FileOperationResult.Success,
                        Exception = null
                    };
                }

                Debug.LogError("DeleteError: file not found");
                return new DeleteCallback()
                {
                    Callback = FileOperationResult.NotFound,
                    Exception = null
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteError: {e.Message}");
                return new DeleteCallback()
                {
                    Callback = FileOperationResult.Error,
                    Exception = e
                };
            }
        }

        public bool DeleteFolder(string[] folders)
        {
            try
            {
                var path = SaveStatic.BuildFoldersPath(folders);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteError: {e.Message}");
                return false;
            }
        }

        public bool DeleteFolder(string folder1, string folder2)
        {
            return DeleteFolder(new[] { folder1, folder2 });
        }
    }
}