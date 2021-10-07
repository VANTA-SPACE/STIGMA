using UnityEngine.Events;

public static class Events {
    public static UnityEvent OnLanguageChange = new UnityEvent();
    public static UnityEvent OnApplicationQuit = new UnityEvent();
    public static UnityEvent OnSettingsUpdate = new UnityEvent();
}