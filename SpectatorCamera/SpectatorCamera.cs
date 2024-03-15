using System;
using System.Collections;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace LibOnward.SpectatorCamera;

public static class SPCamera
{
    private static readonly object _lock = new object();
    
    internal static IEnumerator internal_handleSpectatorCameraDownload(int fov, float smooth)
    {
        Logger.At("SpectatorCamera::internal_handleSpectatorCameraDownload");
        
        var saveTo = Path.Combine(MelonEnvironment.ModsDirectory, "libonward_assets") + "\\spec_camera";

        lock (_lock)
        {
            if (GameObject.Find("Spectator Camera(Clone)") != null)
            {
                Logger.Warn("Spectator Camera already exists, skipping creation");
                yield break;
            }
        
            if (!File.Exists(saveTo))
            {
                var url = "dooomsickle.github.io/LibOnward/bundles/spec_camera";

                var req = UnityWebRequest.Get(url);
        
                yield return req.SendWebRequest();
        
                if (req.isNetworkError || req.isHttpError)
                    Logger.Except<Exception>($"Failed to download spectator camera bundle: {req.error}");
        
                File.WriteAllBytes(saveTo, req.downloadHandler.data);
            }
        
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(saveTo);
            if (myLoadedAssetBundle == null)
                Logger.Except<Exception>("Failed to load AssetBundle");

            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("Spectator Camera.prefab");
            var go = Object.Instantiate(prefab);

            myLoadedAssetBundle.Unload(false);
            go.AddComponent<SmoothFollowBehavior>().smooth = smooth;
            go.GetComponentInChildren<Camera>().cameraType = CameraType.SceneView;
            go.GetComponentInChildren<Camera>().fieldOfView = fov;
        }
    }

    public static void Create(int fov = 90, float smooth = 0.5f)
    {
        Logger.At("SpectatorCamera::Create");

        if (GameObject.Find("Spectator Camera(Clone)") != null)
        {
            Logger.Warn("Spectator Camera already exists, skipping creation");
            return;
        }

        Logger.Log("Creating Spectator Camera");
        
        MelonCoroutines.Start(internal_handleSpectatorCameraDownload(fov, smooth));
    }
}