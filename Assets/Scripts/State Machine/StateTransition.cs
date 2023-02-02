using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class StateTransition
{
    public string transitionName;
    private int priority = 0;
    private StateMachine stateMachine;
    [SerializeField]
    private State targetState;
    [SerializeField]
    private State originState;
    private List<Func<bool>> guards;
    private List<Action> transitionActions;
    [SerializeField]
    private string test;

    //Constructor
    public StateTransition(State originState, State targetState, Func<bool>[] conditions,
        Action[] actions, StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        transitionActions.AddRange(actions);
        guards.AddRange(conditions);
        this.originState = originState;
        this.targetState = targetState;
        transitionName = originState.GetName() + " ---> " + targetState.GetName();
    }

    //Set the priority of this transition
    public void SetPriority(int newPriority)
    {
        priority = newPriority;
    }

    //Get the priority index of this transition
    public int GetPriority()
    {
        return priority;
    }

    //Get the target state from this transition
    public State GetTargetState()
    {
        return targetState;
    }

    //Get the origin state from this transition
    public State GetOriginState()
    {
        return originState;
    }

    //Add a guard to the transition
    public void AddGuard(Func<bool> newCondition)
    {
        guards.Add(newCondition);
    }

    //Add an action to the transition
    public void AddACtion(Action newAction)
    {
        transitionActions.Add(newAction);
    }

    //If all guards return true, condition is met
    public bool Condition()
    {
        return guards.All(guard => guard());
    }

    //Fire the transition -> trigger all actions and enter the target state
    public void FireTransition()
    {
        transitionActions.ForEach(action => action());
        targetState.Enter();
    }

    //Transition is referenced in State machine, need to dereference it for garbage collec
    public void Release()
    {
        try
        {
            targetState = null;
            originState = null;
            stateMachine.DeleteTransition(this);
            stateMachine = null;

        } catch (UnityException e)
        {
            throw new Exception(e.Message);
        }
    }
}
