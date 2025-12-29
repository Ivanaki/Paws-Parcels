using System;
using System.IO;
using Newtonsoft.Json;
using R3;
using UnityEngine;


namespace SaveLaod
{
    public class JsonToFileLoadSaveService : ILoadSaveService
    {
        private JsonSerializerSettings _settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        
        public Observable<bool> Save(string key, object data)
        {
            var path = BuildPath(key);
            var json = JsonConvert.SerializeObject(data);

            using (var fileStream = new StreamWriter(path))
            {
                fileStream.Write(json);
            }

            //Debug.Log("Save");
            
            return Observable.Return(true);
        }

        public Observable<LoadCallback> Load<T>(string key)
        {
            var path = BuildPath(key);

            if (File.Exists(path))
            {
                using (var fileStream = new StreamReader(path))
                {
                    var json = fileStream.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<T>(json);

                    //Debug.Log("LoadComplete");
                    
                    return Observable.Return(new LoadCallback()
                    {
                        Value = data,
                        Callback = SavesStateEnum.Complete
                    });
                }

            }

            //Debug.Log("LoadError");
            
            return Observable.Return(new LoadCallback()
            {
                Callback = SavesStateEnum.FileWithKeyDontExists
            });
        }

        public Observable<bool> Delete(string key)
        {
            var path = BuildPath(key);

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Debug.Log("DeleteComplete");
                    return Observable.Return(true);
                }
                else
                {
                    Debug.Log("DeleteError: file not found");
                    return Observable.Return(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteError: {e}");
                return Observable.Return(false);
            }
        }

        private string BuildPath(string key) => Path.Combine(Application.persistentDataPath, key);
    }
}