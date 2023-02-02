using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Observable : MonoBehaviour
{
    protected List<IObserver> observers;
    protected void AddObserver(IObserver observer) {
        observers.Add(observer);
    }

    protected void RemoveObserver(IObserver observer) {
        observers.Remove(observer);
    }

    protected void NotifyObservers(Action aEvent, EventType type) {
        observers.ForEach(observer => observer.ReceiveEvent(aEvent, type));
    }
}
