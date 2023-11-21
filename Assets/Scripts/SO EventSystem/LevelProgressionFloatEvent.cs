using UnityEngine;
using UnityEngine.Events;

public class LevelProgressionFloatEvent : MonoBehaviour
{
    public float multiplier = 10;

    public UnityEvent<string> InvokeEvent = new UnityEvent<string>();

    public void Invoke(float f)
    {
        InvokeEvent.Invoke(((f * multiplier)).ToString());

    }
}
