using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class State : Observable
{
    private string stateName;
    private readonly Guid uniqueId;
    private StateMachine stateMachine;

    public State(string name)
    {
        uniqueId = Guid.NewGuid();
        stateName = name; 
    }

    public void Bind(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public string GetName()
    {
        return stateName;
    }

    public void SetName(string newName)
    {
        stateName = newName;
    }

    public Guid GetUniqueId()
    {
        return uniqueId;
    }

    public List<StateTransition> GetOutBoundTransitions()
    {
        return stateMachine.GetAllTransitions()
            .Where(transition => transition.GetOriginState().Equals(this))
            .ToList();
    }
    public List<StateTransition> GetInBoundTransitions()
    {
        return stateMachine.GetAllTransitions()
            .Where(transition => transition.GetTargetState().Equals(this))
            .ToList();
    }

    public void Delete()
    {
        GetInBoundTransitions().Concat(GetOutBoundTransitions())
            .Where(transition => transition.GetTargetState().Equals(this) || transition.GetOriginState().Equals(this))
            .ToList()
            .ForEach(transition => transition.Release());
        try
        {
            Destroy(this);
        }
        catch(UnityException e)
        {
            throw new Exception(e.Message);
        }
    }

    public static State CreateState(StateType stateType, string stateName)
    {
        var parameters = new object[] { stateName };
        string className = stateType.ToString() + "State";
        Type type = Type.GetType(className);
        if (type != null && typeof(MonoBehaviour).IsAssignableFrom(type))
        {
            GameObject gameObject = (new GameObject());
            MonoBehaviour monoBehaviour = gameObject.AddComponent(type) as State;
            monoBehaviour.GetType()
                .GetConstructor(parameters.Select(p => p.GetType()).ToArray())?
                .Invoke(monoBehaviour, parameters);
            monoBehaviour.name = ((State)monoBehaviour).GetName();
            return (State)monoBehaviour;
        }
        else return null;
    }

    public virtual void Enter() 
    {
        stateMachine.UpdateSM(this);
    }

    public virtual void Execute() { }

    public virtual void Exit() { }  
}

public enum StateType
{
    Basic //add more types of states if needed
}
