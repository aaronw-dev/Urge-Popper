using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[CreateAssetMenu(fileName = "New Void Event", menuName = "Game Events/Base Event")]
public abstract class BaseGameEvent<T> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<IGameEventListener<T>> eventListeners =
        new List<IGameEventListener<T>>();
    private readonly List<UnityEvent<T>> unityeventListeners = new List<UnityEvent<T>>();

    [HideInInspector]
    public UnityEvent<T> OnRaise;
    [ReadOnly] public T m_previousValue;
    [ReadOnly] public string m_currentID = "";
    public void Raise(T item, string ID = "")
    {
        m_currentID = ID;
        m_previousValue = item;
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(item);
        for (int i = unityeventListeners.Count - 1; i >= 0; i--)
            unityeventListeners[i]?.Invoke(item);
        OnRaise?.Invoke(item);
    }

    public void RegisterListener(IGameEventListener<T> listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }
    public void RegisterListener(UnityEvent<T> listener)
    {
        if (!unityeventListeners.Contains(listener))
            unityeventListeners.Add(listener);
    }

    public void UnregisterListener(IGameEventListener<T> listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }

    public void UnregisterListener(UnityEvent<T> listener)
    {
        if (unityeventListeners.Contains(listener))
            unityeventListeners.Remove(listener);
    }
}
