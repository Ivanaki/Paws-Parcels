using System;
using System.IO;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveLaod
{
    public class ScreenshotSaver
    {
        public Observable<bool> CaptureScreenshot(string key, Camera camera)
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

            return Observable.Return(true);
        }

        public ScreenShotCallback LoadTexture(string key)
        {
            var path = BuildPath(key + "Screenshot" + ".png");
            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(data);
                return new ScreenShotCallback(true, texture);
            }
            Debug.LogError("Load texture failed");
            return new ScreenShotCallback(false, null);
        }

        public Observable<bool> DeleteScreenshot(string key)
        {
            var path = BuildPath(key + "Screenshot" + ".png");

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
        
        private string BuildPath(string key) => Path.Combine(Application.persistentDataPath, "Screenshots", key);
    }
}