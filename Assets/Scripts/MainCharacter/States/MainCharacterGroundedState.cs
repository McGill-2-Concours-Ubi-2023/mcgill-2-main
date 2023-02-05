using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

using static Unity.Mathematics.math;


public class MainCharacterGroundedState : GenericStateMachineBehaviour<MainCharacterGroundedStateBehaviour>
{
}

public class MainCharacterGroundedStateBehaviour : GenericStateMachineMonoBehaviour
{
    private Animator m_Animator;
    private readonly static int Speed = Animator.StringToHash("Speed");
    private float3 m_Input;
    private InputActionAsset m_InputActionAsset;
    private MainCharacterController m_MainCharacterController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Animator = animator;
        PlayerInput input = GetComponent<PlayerInput>();
        m_InputActionAsset = input.actions;
        m_MainCharacterController = GetComponent<MainCharacterController>();
    }
    
    private void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        m_Animator.SetFloat(Speed, controller.velocity.magnitude, 0.1f, Time.deltaTime);
        controller.Move(m_Input * Time.deltaTime);
        
        float forward = m_InputActionAsset["Forward"].ReadValue<float>();
        m_Input.z = forward;
        float left = m_InputActionAsset["Left"].ReadValue<float>();
        m_Input.x = left;
        float3 cameraForward = Vector3.ProjectOnPlane(m_MainCharacterController.Camera.transform.forward, Vector3.up);
        float3 cameraRight = m_MainCharacterController.Camera.transform.right;
        float3 adjustedDirection = m_Input.x * cameraRight + m_Input.z * cameraForward;
        controller.Move(Time.deltaTime * m_MainCharacterController.MovementSpeed * adjustedDirection);
        
        // rotate player to face direction of movement
        if (m_Input.x != 0 || m_Input.z != 0)
        {
            float3 direction = normalize(adjustedDirection);
            float3 forwardVector = new float3(0, 0, 1);
            float angle = -acos(dot(direction, forwardVector));
            float3 cross = math.cross(direction, forwardVector);
            if (cross.y < 0)
            {
                angle = -angle;
            }
            transform.rotation = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);
        }
    }
    
    private void OnFootstep()
    {
        
    }
}
