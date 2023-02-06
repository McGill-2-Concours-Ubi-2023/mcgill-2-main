using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;

public class MainCharacterFreeFallState : GenericStateMachineBehaviour<MainCharacterFreeFallStateBehaviour>
{
    
}

public class MainCharacterFreeFallStateBehaviour : GenericStateMachineMonoBehaviour
{
    private readonly static int FreeFallShouldLand = Animator.StringToHash("FreeFallShouldLand");
    private float3 m_Velocity;
    private readonly static int GroundedToFreeFall = Animator.StringToHash("GroundedToFreeFall");
    private InputActionAsset m_InputActionAsset;
    private MainCharacterController m_MainCharacterController;

    private void Start()
    {
        m_MainCharacterController = GetComponent<MainCharacterController>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(GroundedToFreeFall);
        PlayerInput input = GetComponent<PlayerInput>();
        m_InputActionAsset = input.actions;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!shouldUpdate) return;
        MainCharacterFreeFallCheckAction freeFallCheckAction = new MainCharacterFreeFallCheckAction();
        if (!freeFallCheckAction.ShouldTransition(animator.gameObject))
        {
            Transition(FreeFallShouldLand);
            return;
        }
    }

    private void Update()
    {
        if (!shouldUpdate) return;
        float3 input;
        float forward = m_InputActionAsset["Forward"].ReadValue<float>();
        input.z = forward;
        float left = m_InputActionAsset["Left"].ReadValue<float>();
        input.x = left;
        
        float3 cameraForward = Vector3.ProjectOnPlane(m_MainCharacterController.Camera.transform.forward, Vector3.up);
        float3 cameraRight = m_MainCharacterController.Camera.transform.right;
        float3 adjustedDirection = input.x * cameraRight + input.z * cameraForward;
        m_Velocity.xz = adjustedDirection.xz;

        CharacterController controller = GetComponent<CharacterController>();
        m_Velocity += (float3) Physics.gravity * Time.deltaTime;
        
        controller.Move(m_Velocity * Time.deltaTime);
        
        // rotate player to face direction of movement
        if (input.x != 0 || input.z != 0)
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
}

