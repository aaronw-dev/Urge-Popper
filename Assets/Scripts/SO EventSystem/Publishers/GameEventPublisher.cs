using NaughtyAttributes;
using UnityEngine;

public class GameEventPublisher : MonoBehaviour
{
    [Required] [SerializeField] GameEvent Event = null;
    public bool onlyOnce = false;
    bool shot = false;
    public void _Invoke()
    {
        if (shot) return;
        Event.Raise();
        if (onlyOnce)
            shot = true;
    }
}
