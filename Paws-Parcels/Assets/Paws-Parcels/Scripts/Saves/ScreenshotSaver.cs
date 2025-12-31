using System;
using System.IO;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveLaod
{
    public class ScreenshotSaver
    {
        public class ScreenShotCallback
        {
            public bool IsLoaded;
            public Texture2D Texture;
        
            public ScreenShotCallback(bool isLoaded, Texture2D texture)
            {
                IsLoaded = isLoaded;
                Texture = texture;
            }
        }
        
        public bool CaptureScreenshot(string key, Camera camera)
        {
            try
            {
                int w = Screen.width / 4;
                int h = Screen.height / 4;

                RenderTexture rt = new RenderTexture(w, h, 24);
                camera.targetTexture = rt;

                Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);

                camera.Render();
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                tex.Apply();

                camera.targetTexture = null;
                RenderTexture.active = null;
                Object.Destroy(rt);

                var screenshotPath = BuildPath(key + "Screenshot.png");

                // Создаем папку, если она не существует
                string directory = Path.GetDirectoryName(screenshotPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(screenshotPath, tex.EncodeToPNG());

                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"LoadTextureError: {e}");
                return false;
            }
        }

        public ScreenShotCallback LoadTexture(string key)
        {
            try
            {
                var path = BuildPath(key + "Screenshot" + ".png");
                if (File.Exists(path))
                {
                    byte[] data = File.ReadAllBytes(path);
                    var texture = new Texture2D(2, 2);
                    texture.LoadImage(data);
                    return new ScreenShotCallback(true, texture);
                }
                
                Debug.LogError($"LoadTextureError: file not found");
                return new ScreenShotCallback(false, null);
            }
            catch (Exception e)
            {
                Debug.Log($"LoadTextureError: {e}");
                return new ScreenShotCallback(false, null);
            }
        }

        public bool DeleteScreenshot(string key)
        {
            try
            {
                var path = BuildPath(key + "Screenshot" + ".png");
                
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Debug.Log("DeleteComplete");
                    return true;
                }

                Debug.Log("DeleteError: file not found");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteError: {e}");
                return false;
            }
        }
        
        private string BuildPath(string key) => Path.Combine(Application.persistentDataPath, "Screenshots", key);
    }
}