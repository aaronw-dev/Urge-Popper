using UnityEngine;
using UnityEngine.Events;

public class UnityEvent_Delay : MonoBehaviour
{
    public float _delay;
    public UnityEvent _event;

    public void Invoke()
    {
        Timer.Register(_delay, () => { _event?.Invoke(); }, useRealTime: true);
    }
}
