using UnityEngine;

namespace LibOnward;

internal static class Logger
{
    static string tag = "LIBONWARD::DEFAULT";
    
    public static void At(string _tag) => tag = _tag;
    
    public static void Log(string message, Color color) => MelonLoader.MelonLogger.Msg($"[{tag}] {message}", color);

    public static void Log(string message) => MelonLoader.MelonLogger.Msg($"[{tag}] {message}");

    public static void Warn(string message) => MelonLoader.MelonLogger.Warning($"!! WARNING [{tag}] {message}");

    public static void Error(string message) => MelonLoader.MelonLogger.Error($"[{tag}] {message}");
    
    public static void Debug(string message, Color color) => MelonLoader.MelonLogger.Msg($"!! DEBUG [{tag}] {message}", color);

    public static void Debug(string message) => MelonLoader.MelonLogger.Msg($"!! DEBUG [{tag}] {message}");
    
    public static void Except<TException>(string message) where TException : System.Exception => throw (TException)System.Activator.CreateInstance(typeof(TException), $"(in {tag}) {message}");
}