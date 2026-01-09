using System;
using System.IO;
using Paws_Parcels.Scripts.Saves;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveLaod
{
    public class ScreenshotSaver
    {
        private const string Extension = ".png";
        private const string AddedName = "Screenshot";
        
        public struct SaveScreenshotCallback
        {
            public bool IsComplete;
            public Exception Exception;
        }
        
        public class LoadScreenshotCallback
        {
            public Texture2D Texture;
            public FileOperationResult Callback;
            public Exception Exception;
        }
        
        public struct DeleteScreenshotCallback
        {
            public FileOperationResult Callback;
            public Exception Exception;
        }
        
        public SaveScreenshotCallback CaptureScreenshot(string key, Camera camera, string[] folders = null)
        {
            try
            {
                if (camera == null) throw new Exception("camera is null");
                
                key += AddedName;
                
                var tex = CreateScreenshot(camera);
                
                var screenshotPath = SaveStatic.BuildPath(key, Extension, folders);
                SaveStatic.CreateFolders(folders);

                File.WriteAllBytes(screenshotPath, tex.EncodeToPNG());

                return new SaveScreenshotCallback()
                {
                    IsComplete = true,
                    Exception = null,
                };
            }
            catch (Exception e)
            {
                Debug.Log($"LoadTextureError: {e}");
                return new SaveScreenshotCallback()
                {
                    IsComplete = false,
                    Exception = e
                };
            }
        }

        

        public LoadScreenshotCallback LoadTexture(string key, string[] folders = null)
        {
            try
            {
                key += AddedName;
                
                var screenshotPath = SaveStatic.BuildPath(key, Extension, folders);
                if (File.Exists(screenshotPath))
                {
                    byte[] data = File.ReadAllBytes(screenshotPath);
                    var texture = new Texture2D(2, 2);
                    texture.LoadImage(data);
                    return new LoadScreenshotCallback()
                    {
                        Callback = FileOperationResult.Success,
                        Texture = texture,
                        Exception = null
                    };
                }
                
                Debug.LogError($"LoadTextureError: file not found");
                return new LoadScreenshotCallback()
                {
                    Callback = FileOperationResult.NotFound,
                    Texture = null,
                    Exception = null
                };
            }
            catch (Exception e)
            {
                Debug.Log($"LoadTextureError: {e}");
                return new LoadScreenshotCallback()
                {
                    Callback = FileOperationResult.Error,
                    Texture = null,
                    Exception = e
                };
            }
        }

        public DeleteScreenshotCallback DeleteScreenshot(string key, string[] folders = null)
        {
            try
            {
                var screenshotPath = SaveStatic.BuildPath(key, Extension, folders);
                
                if (File.Exists(screenshotPath))
                {
                    File.Delete(screenshotPath);
                    return new DeleteScreenshotCallback()
                    {
                        Callback = FileOperationResult.Success,
                        Exception = null
                    };
                }

                Debug.Log("DeleteError: file not found");
                return new DeleteScreenshotCallback()
                {
                    Callback = FileOperationResult.NotFound,
                    Exception = null
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteError: {e}");
                return new DeleteScreenshotCallback()
                {
                    Callback = FileOperationResult.Error,
                    Exception = e
                };
            }
        }
        
        private Texture2D CreateScreenshot(Camera camera)
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
            
            return tex;
        }
    }
}