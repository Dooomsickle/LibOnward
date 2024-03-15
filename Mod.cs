using System;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using LibOnward.SpectatorCamera;
using LibOnward.UI;
using MelonLoader;

[assembly: MelonGame("Downpour Interactive", "Onward")]
[assembly: MelonInfo(typeof(LibOnward.Main), "LibOnward", "1.0.0", "Doomsickle")]

namespace LibOnward;

public class Main : MelonMod
{
    public override void OnInitializeMelon()
    {
        Logger.At("LibOnward::Init");

        try
        {
            ClassInjector.RegisterTypeInIl2Cpp<SmoothFollowBehavior>();
        }
        catch (Exception e)
        {
            Logger.Except<Exception>("[CRITICAL] Failed to inject SmoothFollowBehavior into Il2Cpp: " + e.Message);
        }
        
        Utils.OnLocalPlayerSpawned += () =>
        {
            foreach (var weapon in Utils.CameraRig.GetComponentsInChildren<Pickup_Gun>())
            {
                MelonLogger.Msg("weapon name: " + weapon.name);
                
                weapon.Weapon.ActualROF = 0;
                weapon.Weapon._roundsLoaded = 999999;
                weapon.WeaponSO.HasRecoil = false;
            }
            
            TabletUI.AddButton("highlight players", () => Cheats.HighlightPlayers(true));
            TabletUI.AddButton("unhighlight players", () => Cheats.HighlightPlayers(false));

            TabletUI.AddButton("spawn svd", () =>
            {
                Cheats.SpawnWeapon("SVD", 3);
            });
            TabletUI.AddButton("spawn pkm", () =>
            {
                Cheats.SpawnWeapon("PKM", 3);
            });
            TabletUI.AddButton("spawn m16A4", () =>
            {
                Cheats.SpawnWeapon("M16A4", 3);
            });
            
            TabletUI.AddButton("spectator camera", () =>
            {
                SPCamera.Create();
            });
        };
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (Safety.sceneNamesWeDontLike.Contains(sceneName) || Safety.spawnHandlerInitialized)
            return;
        
        MelonCoroutines.Start(Safety.WaitForLocalPlayer());
        
        SPCamera.Create();
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName) => Safety.spawnHandlerInitialized = false;
}