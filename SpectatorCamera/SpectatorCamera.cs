using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LibOnward.SpectatorCamera;

public static class SPCamera
{
    public static void Create(int fov = 90, float smooth = 0.5f)
    {
        Logger.At("SpectatorCamera::Create");

        if (GameObject.Find("Spectator Camera(Clone)") != null)
        {
            Logger.Warn("Spectator Camera already exists, skipping creation");
            return;
        }

    Logger.Log("Creating Spectator Camera");
        
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(@"C:\Users\sgtdo\My project (1)\MyBuild\meshes");
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