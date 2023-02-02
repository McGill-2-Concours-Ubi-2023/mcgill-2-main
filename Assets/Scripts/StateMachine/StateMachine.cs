using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour, IStateMachine
{
    private List<State> states;
    [SerializeField]
    private List<StateTransition> transitions;
    private State currentState;
    private State baseState;
    public int layer;
    public int layerPosition;

    private void Start()
    {
        AddState("BaseState", StateType.Basic);
        Debug.Log(baseState.GetName());
    }

    public StateMachine(int layer, int layerPosition)
    {
        states = new List<State>();
        this.layer = layer;
        this.layerPosition = layerPosition;
    }

    public void SetLayer(int newLayer)
    {
        layer = newLayer;
    }

    public void SetLayerPosition(int newPosition)
    {
        layerPosition = newPosition;
    }

    public void AddState(string stateName, StateType type)
    {
        State newState = State.CreateState(type, stateName);
        newState.Bind(this);
        states.Add(newState);
        if (states.Count == 1) baseState = newState;
    }

    public void DeleteState(State state)
    {
        states.Remove(state);
        state.Delete();
    }

    public void DeleteTransition(StateTransition transition)
    {
        transitions.Remove(transition);
    }

    public void ChangeBaseState(string stateName)
    {
        baseState = FindState(stateName);
    }

    public State FindState(string stateName)
    {
        return states.Where(transition => transition.GetName().Equals(stateName))
            .First();           
    }

    public State GetBaseState()
    {
        return baseState;
    }

    public State FindState(Guid id)
    {
        return states.Where(transition => transition.GetUniqueId().Equals(id))
            .First();
    }

    public void AddTransition(State originState, State targetState, Action[] actions, Func<bool> [] conditions)
    {
        transitions.Add(new StateTransition(originState, targetState, conditions, actions,this));
    }

    public void UpdateSM(State state)
    {
        currentState = state;
    }

    public void CheckTransitions()
    {
        currentState.GetOutBoundTransitions()
            .Where(transition => transition.Condition())
            .OrderBy(transition => transition.GetPriority())
            .Last()
            .FireTransition();
    }

    public ReadOnlyCollection<State> GetAllStates()
    {
        return new ReadOnlyCollection<State>(states);
    }

    public ReadOnlyCollection<StateTransition> GetAllTransitions()
    {
        return new ReadOnlyCollection<StateTransition>(transitions);
    }
}
