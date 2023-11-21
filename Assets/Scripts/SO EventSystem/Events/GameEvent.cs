using UnityEngine;


[CreateAssetMenu(fileName = "New Void Event", menuName = "Game Events/Base Event")]

public class GameEvent : BaseGameEvent<Void>
{
    public void Raise() => Raise(new Void());
}
