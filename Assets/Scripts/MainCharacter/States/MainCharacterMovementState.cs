using System;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;

public class MainCharacterMovementState : GenericStateMachineBehaviour<MainCharacterMovementStateBehaviour>
{
}

public class MainCharacterMovementStateBehaviour : GenericStateMachineMonoBehaviour, IMainCharacterTriggers
{
    private float3 m_MovementIntention;
    private MainCharacterController m_Controller;
    private CharacterController m_CharacterController;
    private Animator m_Animator;
    private readonly static int FreeFallShouldLand = Animator.StringToHash("FreeFallShouldLand");
    private readonly static int Speed = Animator.StringToHash("Speed");

    private void Start()
    {
        m_Controller = GetComponent<MainCharacterController>();
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!shouldUpdate)
        {
            return;
        }
        
        m_Animator.ResetTrigger(FreeFallShouldLand);
        m_Animator.SetFloat(Speed, m_CharacterController.velocity.magnitude, 0.1f, Time.fixedDeltaTime);
        
        // move by velocity
        float speed = m_Controller.MovementSpeed;
        m_CharacterController.Move(speed * m_MovementIntention * Time.fixedDeltaTime);

        if (any(m_MovementIntention.xz != float2.zero))
        {
            float3 direction = normalize(m_MovementIntention);
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

    public void OnMovementIntention(float3 intention)
    {
        m_MovementIntention = intention;
    }

    public void OnFootstep()
    {
        
    }
}
