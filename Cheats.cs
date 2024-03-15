using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppOnward.Data;
using Il2CppOnward.Networking;
using Il2CppOnward.Weapons;
using LibOnward.Equipment;
using LibOnward.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LibOnward;

public static class Cheats
{
    public static void HighlightPlayers(bool enabled)
    {
        foreach (var player in Utils.GetPlayers())
        {
            /*if (player == WarPlayerScript.LocalWarPlayer)
                continue;*/

            player.Outline.SetOutlineActive(enabled);
            player.Outline.SetOutlineColor(player.IsEnemy ? Color.red : Color.blue);
        }
    }

    static List<string> spawnedWeapons = new();
    
    static int newWeaponId = 99998;
    static int newMagazineId = 199998;
    
    /// <summary>
    /// Spawns in a weapon via cloning its inactive prefab.
    /// </summary>
    /// <param name="weaponName">The name of the weapon.
    /// This should be the code-based name of it, such as "MCXVirtus" instead of "MCX Virtus", because it looks for an object starting with "pref_weapon_PC_".</param>
    /// <remarks>The same weapon cannot be spawned in twice.</remarks>
    public static void SpawnWeapon(string weaponName, int magCount = 1)
    {
        Logger.At("LibOnward::Cheats::SpawnWeapon");
        
        if (spawnedWeapons.Contains(weaponName))
            Logger.Except<NotSupportedException>("You cannot spawn in the same weapon twice.");
        
        var weapon = Resources.FindObjectsOfTypeAll<Transform>()
            .FirstOrDefault(x => x.name == "pref_weapon_PC_" + weaponName && x.GetComponent<Weapon>() != null && !x.name.Contains("(Clone)"))?
            .gameObject;
        
        var magazine = Resources.FindObjectsOfTypeAll<Transform>()
            .FirstOrDefault(x => x.name == "pref_magazine_" + weaponName + "-PC" && x.GetComponent<Pickup_Magazine>() != null && !x.name.Contains("(Clone)"))?
            .gameObject;
        
        if (weapon == null)
            Logger.Except<GameObjectNotFoundException>($"Target weapon prefab ({"pref_weapon_PC_" + weaponName}) not found. Are you using the correct name?");
        
        weapon!.SetActive(true);
        
        var photonView = weapon.GetComponent<OnwardPhotonView>();
        photonView.enabled = true;
        
        var spawnedWeapon = Object.Instantiate(weapon);
        spawnedWeapon.hideFlags = HideFlags.None;
        
        spawnedWeapon.transform.position = PlayerVest.VestObject.transform.position;
        
        Logger.Log($"weapon {"pref_weapon_PC_" + weaponName} instantiated at {spawnedWeapon.transform.position}");
        
        photonView.enabled = false;
        weapon.SetActive(false);
        
        spawnedWeapons.Add(weaponName);
        
        var weaponc = spawnedWeapon.GetComponent<Weapon>();
        weaponc.Init();
        weaponc.WeaponSO.Initialize();
        
        var pickup = spawnedWeapon.GetComponent<Pickup_Gun>();
        pickup.InitGun();
        pickup.InitPickup();
        pickup.InitializeEquipActions();

        spawnedWeapon.GetComponent<OnwardPhotonView>().enabled = true;
        spawnedWeapon.GetComponent<OnwardPhotonView>().ViewId = newWeaponId++;
        spawnedWeapon.GetComponent<OnwardPhotonView>().NetworkInitialized();
        
        spawnedWeapon.transform.position = Utils.CameraRig.transform.Find("Camera (eye)").position;
        
        magazine.SetActive(true);
        var photonViewMag = magazine.GetComponent<OnwardPhotonView>();
        photonViewMag.enabled = true;
        for (int i = 0; i < magCount; i++)
        {
            var mag = Object.Instantiate(magazine);
            mag.hideFlags = HideFlags.None;
            
            var phtnViewMag = mag.GetComponent<OnwardPhotonView>();
            phtnViewMag.enabled = true;
            phtnViewMag.ViewId = newMagazineId++;
            phtnViewMag.NetworkInitialized();
            
            var pickupMag = mag.GetComponent<Pickup_Magazine>();

            var dummyMag = new Magazine(pickupMag.weaponSO.MagazineCapacity);
            dummyMag.Ammo = dummyMag.Capacity;
            pickupMag._magazine = dummyMag;
            
            mag.transform.position = Utils.CameraRig.transform.Find("Camera (eye)").position;
        }

        photonViewMag.enabled = false;
        magazine.SetActive(false);
    }
}