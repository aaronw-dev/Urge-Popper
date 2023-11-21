using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class MonobehaviourEvents : MonoBehaviour
{
    public UnityEvent OnAwake;
    public UnityEvent OnStart;
    [Label("On Enable")] public UnityEvent Onenable;
    [Label("On Disable")] public UnityEvent Ondisable;
    [Label("On Application Quit")] public UnityEvent OnapplicationQuit;
    public Platform TargetedPlatform;
    private void Awake()
    {
        if (TargetedPlatform == Platform.OnlyEditor)
        {
#if UNITY_EDITOR
            OnAwake?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.NotEditor)
        {
#if !UNITY_EDITOR
            OnAwake?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.Both)
        {
            OnAwake?.Invoke();
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if (TargetedPlatform == Platform.OnlyEditor)
        {
#if UNITY_EDITOR
            OnStart?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.NotEditor)
        {
#if !UNITY_EDITOR
            OnStart?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.Both)
        {
            OnStart?.Invoke();
        }

    }

    private void OnDisable()
    {
        if (TargetedPlatform == Platform.OnlyEditor)
        {
#if UNITY_EDITOR
            Ondisable?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.NotEditor)
        {
#if !UNITY_EDITOR
            Ondisable?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.Both)
        {
            Ondisable?.Invoke();
        }

    }

    void OnEnable()
    {
        if (TargetedPlatform == Platform.OnlyEditor)
        {
#if UNITY_EDITOR
            Onenable?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.NotEditor)
        {
#if !UNITY_EDITOR
            Onenable?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.Both)
        {
            Onenable?.Invoke();
        }

    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            OnApplicationQuit();
    }
    private void OnApplicationQuit()
    {
        if (TargetedPlatform == Platform.OnlyEditor)
        {
#if UNITY_EDITOR
            OnapplicationQuit?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.NotEditor)
        {
#if !UNITY_EDITOR
            OnapplicationQuit?.Invoke();
#endif
        }
        else if (TargetedPlatform == Platform.Both)
        {
            OnapplicationQuit?.Invoke();
        }
    }
    public enum Platform
    {
        OnlyEditor = 1, NotEditor = 2, Both = 0
    }
}
