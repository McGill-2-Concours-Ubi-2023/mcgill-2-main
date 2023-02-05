using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;


public class MainCharacterGroundedState : GenericStateMachineBehaviour<MainCharacterGroundedState, MainCharacterGroundedStateBehaviour>
{
}

public class MainCharacterGroundedStateBehaviour : GenericStateMachineMonoBehaviour
{
    private Animator m_Animator;
    private readonly static int Speed = Animator.StringToHash("Speed");
    private float3 m_Input;
    private InputActionAsset m_InputActionAsset;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Animator = animator;
        PlayerInput input = GetComponent<PlayerInput>();
        m_InputActionAsset = input.actions;
    }
    
    private void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        m_Animator.SetFloat(Speed, controller.velocity.magnitude);
        controller.Move(m_Input * Time.deltaTime);
        
        float value = m_InputActionAsset["Forward"].ReadValue<float>();
        m_Input.z = value;
        controller.Move(m_Input * Time.deltaTime);
    }
}
