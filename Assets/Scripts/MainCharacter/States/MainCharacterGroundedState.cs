using System;
using UnityEngine;
using UnityEngine.Animations;


public class MainCharacterGroundedState : GenericStateMachineBehaviour<MainCharacterGroundedState, MainCharacterGroundedStateBehaviour>
{
}

public class MainCharacterGroundedStateBehaviour : GenericStateMachineMonoBehaviour
{
    private void OnForward()
    {
        Debug.Log("Forward");
    }
}
