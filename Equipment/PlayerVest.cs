using UnityEngine;

namespace LibOnward.Equipment;

public static class PlayerVest
{
    public static GameObject VestObject =>
        Utils.CameraRig.transform.Find("BodyPosition/pref_char_player(Clone)/" +
                                       "IKBody_VestPivot/IKBody_LocalVest/VestDisabledOnDeadOrDowned/").gameObject;

    public static GameObject RightHandPrimaryHolster => VestObject.transform.Find("UpperVestObjects/Holster_Primary_RightHanded/Pivot").gameObject;

    public static GameObject LeftHandPrimaryHolster => VestObject.transform.Find("UpperVestObjects/Holster_Primary_LeftHanded/Pivot").gameObject;

    public static GameObject RightHandSecondaryHolster => VestObject.transform.Find("LowerVestObjects/Holster_Secondary_RightHip/Pivot").gameObject;

    public static GameObject LeftHandSecondaryHolster => VestObject.transform.Find("LowerVestObjects/Holster_Secondary_LeftHip/Pivot").gameObject;
}