using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntString Event", menuName = "Game Events/ IntString Event")]
public class IntStringEvent : ScriptableObject
{
 private List<IntStringEventListener> _observers = new List<IntStringEventListener>();
    internal void RegisterObserver(IntStringEventListener observer)
    {
        _observers.Add(observer);
    }

    internal void UnregisterObserver(IntStringEventListener observer)
    {
        _observers.Remove(observer);
    }

    public void Occurred(int dps, string element)
    {
        foreach (var observer in _observers)
        {
            observer.OnEventOccurred(dps, element);
        }
    }
}
