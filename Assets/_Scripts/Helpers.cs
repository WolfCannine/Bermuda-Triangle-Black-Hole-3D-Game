using System;
using UnityEngine.Events;

public static class Helpers
{
    public static void SomeThing() { }
}

[Serializable]
public class EventAndResponse
{
    public string name;
    public GameEvent gameEvent;
    public UnityEvent genericResponse;
    public UnityEvent<int> sentIntResponse;
    public UnityEvent<bool> sentBoolResponse;
    public UnityEvent<float> sentFloatResponse;
    public UnityEvent<string> sentStringResponse;

    public void EventRaised()
    {
        if (genericResponse.GetPersistentEventCount() >= 1) { genericResponse.Invoke(); }

        if (sentIntResponse.GetPersistentEventCount() >= 1) { sentIntResponse.Invoke(gameEvent.sentInt); }

        if (sentBoolResponse.GetPersistentEventCount() >= 1) { sentBoolResponse.Invoke(gameEvent.sentBool); }

        if (sentFloatResponse.GetPersistentEventCount() >= 1) { sentFloatResponse.Invoke(gameEvent.sentFloat); }

        if (sentStringResponse.GetPersistentEventCount() >= 1) { sentStringResponse.Invoke(gameEvent.sentString); }
    }
}

[Serializable]
public enum EatableType
{
    none, redBoat, largeBoat, motorBoat, speedBoat, fishingBoat, small2seatBoat, shark, dolphin,
    starFish, killFish, swoodFish, largeShark, cartoonFish, estrala, octopus, tourtle, seaHourse, island,
}
