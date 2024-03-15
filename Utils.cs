using System;
using System.Collections;
using System.Linq;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using LibOnward.UI;
using MelonLoader;
using UnityEngine;

namespace LibOnward;

public static class Utils
{
    public static GameObject CameraRig => GameObject.Find("CameraRig_SteamVR(Clone)");
    
    public static Il2CppArrayBase<WarPlayerScript> GetPlayers() => Resources.FindObjectsOfTypeAll<WarPlayerScript>();

    /// <summary>
    /// Executed when the player spawns in.
    /// </summary>
    public static event Action OnLocalPlayerSpawned;

    // external event raising
    internal static void InvokeOnLocalPlayerSpawned()
    {
        TabletUI.Init();
        
        OnLocalPlayerSpawned?.Invoke();
    } 
}
