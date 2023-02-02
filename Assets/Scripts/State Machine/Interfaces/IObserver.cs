using System;

public interface IObserver 
{
    //Handle event
    public void ReceiveEvent(Action aEvent, EventType type);
}

public enum EventType
{
    TIME_SENSITIVE, ACTION_SENSITIVE //TODO: add event types as needed
}
