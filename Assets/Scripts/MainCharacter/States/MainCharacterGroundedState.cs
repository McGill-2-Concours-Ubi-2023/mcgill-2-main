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
    private readonly static int GroundedToFreeFall = Animator.StringToHash("GroundedToFreeFall");
    private readonly static int GroundedToJumpAction = Animator.StringToHash("GroundedToJumpAction");
    private readonly static int FreeFallShouldLand = Animator.StringToHash("FreeFallShouldLand");
    private float m_TimeOnEnter;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Animator = animator;
        PlayerInput input = GetComponent<PlayerInput>();
        m_InputActionAsset = input.actions;
        m_MainCharacterController = GetComponent<MainCharacterController>();
        animator.ResetTrigger(FreeFallShouldLand);
        m_TimeOnEnter = Time.time;
    }

    private void Update()
    {
        if (!shouldUpdate)
        {
            return;
        }
        
        m_Animator.ResetTrigger(FreeFallShouldLand);
        CharacterController controller = GetComponent<CharacterController>();
        m_Animator.SetFloat(Speed, controller.velocity.magnitude, 0.1f, Time.deltaTime);
        controller.Move(m_Input * Time.deltaTime);

        float2 rawInput = m_InputActionAsset["Movement"].ReadValue<Vector2>();

        m_Input.xz = rawInput.xy;

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
            
            MainCharacterJumpToPlatformAction jumpAction = new MainCharacterJumpToPlatformAction();
            jumpAction.MaxDistance = 2.0f;
            if (jumpAction.ShouldTransition(gameObject))
            {
                Vector3 jumpTarget = jumpAction.Hit.point;
                //controller.Move(jumpTarget - controller.transform.position);
                gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.SetNavActionData), jumpTarget);
                Transition(GroundedToJumpAction);
                return;
            }
        }

        // suppress free fall transition for a short time after entering this state
        if (Time.time - m_TimeOnEnter > 0.3f)
        {
            MainCharacterFreeFallCheckAction freeFallCheck = new MainCharacterFreeFallCheckAction();
            if (freeFallCheck.ShouldTransition(gameObject))
            {
                Transition(GroundedToFreeFall);
                return;
            }
        }
    }
    
    private void OnFootstep()
    {
        
    }
}
