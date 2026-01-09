using System;
using System.Collections.Generic;
using Saves;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class TestMO : MonoBehaviour
    {
        [System.Serializable]
        private class Data1
        {
            public string data1;
        }
        
        [Inject]
        private JsonFileSerializer _jsonFileSerializer;

        private void OnEnable()
        {
            print("OnEnable");
        }

        private void Awake()
        {
            print("Awake");
        }

        private void Start()
        {
            print("Start");
            
            /*var data = new Dictionary<string, object>
            {
                ["test1"] = 1,
                ["test2"] = new Data1()
            };
            var callback = _jsonFileSerializer.Save(data, "data", new []{"g", "i", "m"});
            if (callback.IsComplete)
            {
                print("Complete");
            }

            if (!callback.IsComplete)
            {
                print("Failed");
                print(callback.Exception);
            }

            var loadCallback = _jsonFileSerializer.Load<Dictionary<string, object>>("data", new []{"g", "i", "m"});
            if (loadCallback.Callback == JsonFileSerializer.LoadDeleteStateEnum.Complete)
            {
                print("CompleteLoad");
                print(loadCallback.Value);
            }
            else if (loadCallback.Callback == JsonFileSerializer.LoadDeleteStateEnum.FileWithKeyDontExists)
            {
                print("not found");
            }
            else if(loadCallback.Callback == JsonFileSerializer.LoadDeleteStateEnum.Error)
            {
                print(callback.Exception);
            }


            var deleteCallback = _jsonFileSerializer.Delete("data", new[] { "g", "i", "m" });*/

            /*try
            {
                print(JsonFileSerializer.BuildPath("key"));
                print(JsonFileSerializer.BuildPath("key", new []{"folder1", "folder2"}));
            }
            catch (Exception e)
            {
                print(e.Message);
            }*/

            /*var utc = DateTime.UtcNow;
            var moscow = utc.ToLocalTime();
            var test = moscow.ToLocalTime();
            print(utc.ToString(""));
            print(moscow.ToString());
            print(test.ToString());
            print(utc.ToString("o"));
            print(utc.ToString("yyyy-MM-dd HH:mm:ss"));
            print(utc.ToString(""));

            var time = DateTime.Parse(utc.ToString("o"));
            print(time);
            print(time.ToString("yyyy-MM-dd HH:mm:ss"));
            print(time.ToString(""));*/
        }
    }
}