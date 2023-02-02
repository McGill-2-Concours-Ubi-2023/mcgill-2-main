using System;
using System.Collections.ObjectModel;
using UnityEngine;

public interface IStateMachine
{
    public void UpdateSM(State state);
    public void AddState(string stateName, StateType type);
    public void DeleteState(State state);
    public void SetLayer(int newLayer);
    public void SetLayerPosition(int newPosition);
    public void AddTransition(State originState, State targetState, Action[] actions, Func<bool>[] conditions);
    public void CheckTransitions();
    public State FindState(Guid id);
    public State FindState(string stateName);
    public void ChangeBaseState(string stateName);
    public ReadOnlyCollection<State> GetAllStates();
    public ReadOnlyCollection<StateTransition> GetAllTransitions();
}
