using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

[System.Serializable]
public class IntStringUnityEvent : UnityEvent<int, string> { }
public class IntStringEventListener : MonoBehaviour
{
    public IntStringEvent gameEvent;
    public IntStringUnityEvent _eventResponse = new IntStringUnityEvent();

    private void OnEnable()
    {
        gameEvent.RegisterObserver(this);
    }
    private void OnDisable()
    {
        gameEvent.UnregisterObserver(this);
    }

    internal void OnEventOccurred(int value, string element)
    {
        _eventResponse.Invoke(value, element);
    }
}