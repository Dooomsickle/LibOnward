using System;
using System.Linq;
using System.Management.Instrumentation;
using Il2CppInterop.Runtime;
using Il2CppOnward.Tablet;
using Il2CppOnward.UI;
using LibOnward.Equipment;
using LibOnward.Exceptions;
using UnityEngine;
using UnityEngine.Events;

namespace LibOnward.UI;

public static class TabletUI
{
    /// <summary>
    /// The canvas that the tablet uses for its UI.
    /// </summary>
    public static Canvas ScreenCanvas => GearManagement.Tablet.GetComponentInChildren<Pickup_Tablet_Screen_Deployer>().deployee.ScreenController.Canvas;
    
    /// <summary>
    /// Whether the UI has been initialized. Adding shit will not work if it isn't.
    /// </summary>
    public static bool HasInit { get; internal set; } = false;
    
    internal static GameObject _buttonPrefab;
    internal static Onward_UI_LayoutGroup _layoutGroup;

    /// <summary>
    /// The amount of buttons instantiated via mods. Mainly used for determining where the next one should be placed.
    /// </summary>
    public static int ButtonCount { get; private set; } = 0;

    /// <summary>
    /// Creates a button on the tablet. This should be called in an OnLocalPlayerSpawned event, as it gets reset when the player dies.
    /// </summary>
    /// <param name="text">The text to display on the button.</param>
    /// <param name="onClick">Callback that gets ran when the button is clicked.</param>
    /// <returns>A reference to the button's <see cref="UnityEngine.GameObject"/>.</returns>
    /// <exception cref="NotSupportedException">when the UI is not initialized first.</exception>
    public static GameObject AddButton(string text, Action onClick)
    {
        Logger.At("LibOnward::TabletUI::AddButton");

        if (!HasInit)
            Logger.Except<NotSupportedException>("UI must be initialized before adding elements. You may be trying to add a button before the player has spawned, or before LibOnward runs its own initialization.");
        
        var button = UnityEngine.Object.Instantiate(_buttonPrefab, _layoutGroup.transform);
        button.SetActive(true);
        
        button.name = "Button " + ButtonCount;
        button.transform.position = _buttonPrefab.transform.position;

        button.transform.localPosition = new Vector3
        (
            -155 + 100 * (ButtonCount % 3),
            (float)(-150f - 20f * Math.Floor(ButtonCount / 3f))
        );
        
        button.transform.rotation = _buttonPrefab.transform.rotation;
        button.transform.localScale = _buttonPrefab.transform.localScale;
        
        var buttonComponent = button.GetComponent<Onward_UI_Sprite_Button>();
        var textComponent = button.GetComponentInChildren<Il2CppTMPro.TextMeshPro>();
        
        textComponent.text = text;
        
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(onClick));
        
        ButtonCount++;
        
        return button;
    }
    
    internal static void Init()
    {
        Logger.At("LibOnward::TabletUI::Init");
        
        if (HasInit)
        {
            Logger.Warn("UI already initialized, skipping");
            return;
        }
        
        _layoutGroup = ScreenCanvas.GetComponentsInChildren<Onward_UI_LayoutGroup>(true).FirstOrDefault(group => group.name == "Button LayoutGroup");
        
        if (_layoutGroup == null) 
            Logger.Except<GameObjectNotFoundException>("No Onward_UI_LayoutGroup found under the tablet canvas");

        _buttonPrefab = _layoutGroup.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(comp => comp.name == "Button Refresh Loadout")?.gameObject;
        
        if (_buttonPrefab == null)
            Logger.Except<GameObjectNotFoundException>("No button prefab found under the tablet canvas");
        
        HasInit = true;
        Logger.Log("UI initialized", Color.green);
    }
}