using System.Collections;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime;
using LibOnward.Equipment;
using LibOnward.UI;
using MelonLoader;
using UnityEngine;

namespace LibOnward;

internal class Safety
{
    // scenes where we don't need to wait for the player to spawn
    internal static List<string> sceneNamesWeDontLike = new() { "main_menu_fob", "splishsplash", "DownpourSplashScreen", "LoadingScene_Quest" };
    
    internal static bool spawnHandlerInitialized;
    
    internal static object lockObject = new();
    
    // unfortunately we can't add a hook for when the player spawns, so we have to poll for it
    internal static IEnumerator WaitForLocalPlayer()
    {
        Logger.At("LibOnward::Safety::WaitForLocalPlayer");
        
        // make sure the player actually exists
        while (Utils.CameraRig == null || GameObject.Find("CameraRig_SteamVR(Clone)")?.active == false || WarPlayerScript.LocalWarPlayer == null)
            yield return null;
        
        // for callbacks that need weapon references
        while (Utils.CameraRig.GetComponentsInChildren<Pickup_Gun>() == null)
            yield return null;
        
        // for the tablet ui
        while (GearManagement.Tablet == null)
            yield return null;

        // never used lock before, but i think this is the right way to use it
        // for some reason this coroutine likes to run three times at once :(
        lock (lockObject)
        {
            if (spawnHandlerInitialized)
                yield break;
            
            // add a despawn handler for the local player instead of a spawn hook because the player gets garbage collected when they die
            WarPlayerScript.LocalWarPlayer.add_PlayerDespawned(DelegateSupport.ConvertDelegate<WarPlayerScript.PlayerEvent>(internal_handlePlayerDespawn));
    
            Logger.Log("despawn hook added");
            
            spawnHandlerInitialized = true;
            
            Utils.InvokeOnLocalPlayerSpawned();
        }
    }
    
    // when the player dies we lose our reference to the local player because it gets garbage collected
    // basically we need to re-find the player every time they die
    internal static void internal_handlePlayerDespawn()
    {
        Logger.At("LibOnward::Safety::internal_handlePlayerDespawn");
        
        spawnHandlerInitialized = false;
        TabletUI.HasInit = false;
        
        Logger.Log("warplayer despawned, polling again");
        MelonCoroutines.Start(WaitForLocalPlayer());
    }

    // do the exact same thing as when we despawn
    internal static void internal_handleLoadoutReset()
    {
        Logger.At("LibOnward::Safety::internal_handleLoadoutReset");
        
        spawnHandlerInitialized = false;
        TabletUI.HasInit = false;
        
        Logger.Log("loadout reset, polling again");
        MelonCoroutines.Start(WaitForLocalPlayer());
    }
}