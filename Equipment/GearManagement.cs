using System.Linq;
using Il2CppOnward.Tablet;
using LibOnward.Exceptions;
using UnityEngine;

namespace LibOnward.Equipment;

public static class GearManagement
{
    public static Pickup_Tablet Tablet
    {
        get
        {
            Logger.At("GearManagement::get_Tablet");
            
            var component = Utils.CameraRig.GetComponentInChildren<Pickup_Tablet>();

            if (component == null)
                Logger.Except<GameObjectNotFoundException>("Tablet not found. Are you sure you're holding it or have it holstered?");

            return component;
        }
    }
}