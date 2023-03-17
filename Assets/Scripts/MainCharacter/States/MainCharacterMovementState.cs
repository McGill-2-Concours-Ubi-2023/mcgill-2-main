using System;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;
using float3 = Unity.Mathematics.float3;

public class MainCharacterMovementState : GenericStateMachineBehaviour<MainCharacterMovementStateBehaviour>
{
}

public class MainCharacterMovementStateBehaviour : GenericStateMachineMonoBehaviour, IMainCharacterTriggers
{
    private float3 m_MovementIntention;
    private float3 m_FaceIntention;
    private MainCharacterController m_Controller;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private readonly static int FreeFallShouldLand = Animator.StringToHash("FreeFallShouldLand");
    private readonly static int Speed = Animator.StringToHash("Speed");
    private readonly static int MovementToDash = Animator.StringToHash("MovementToDash");

    private void Start()
    {
        m_Controller = GetComponent<MainCharacterController>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        // get current face direction
        m_FaceIntention = transform.forward;
    }

    private void FixedUpdate()
    {
        if (!shouldUpdate)
        {
            return;
        }
        
        m_Animator.ResetTrigger(FreeFallShouldLand);
        m_Animator.SetFloat(Speed, m_Rigidbody.velocity.magnitude, 0.1f, Time.fixedDeltaTime);
        
        // move by force
        float speed = m_Controller.MovementSpeed;
        float3 movementIntentionVel = m_MovementIntention * speed;
        movementIntentionVel.y = m_Rigidbody.velocity.y;
        float3 deltaVel = movementIntentionVel - (float3)m_Rigidbody.velocity;
        float3 force = m_Rigidbody.mass * deltaVel / Time.fixedDeltaTime;
        // if force is backward, reduce it
        if (dot(force, movementIntentionVel) <= EPSILON)
        {
            if (length(force) > 5)
            {
                force = normalize(force) * 5;
            }
        }
        
        m_Rigidbody.AddForce(force, ForceMode.Force);
        Debug.DrawRay(transform.position + transform.up, force, Color.red);

        Ref<bool> hasFaceIntention = false;
        gameObject.Trigger<IMainCharacterTriggers, Ref<bool>>(nameof(IMainCharacterTriggers.HasFaceDirectionInput), hasFaceIntention);
        gameObject.Trigger<IMainCharacterTriggers, float3>(nameof(IMainCharacterTriggers.AdjustFaceDirection),
        hasFaceIntention ? m_FaceIntention : m_MovementIntention);
    }

    public void OnMovementIntention(float3 intention)
    {
        m_MovementIntention = intention;
    }

    public void OnFootstep()
    {
        
    }
    
    public void OnDashIntention()
    {
        Transition(MovementToDash);
    }

    public void OnPlayerFaceIntention(float3 intention)
    {
        if (all(intention.xz == float2.zero))
        {
            return;
        }
        m_FaceIntention = intention;
    }

    public void OnSpawnCrateIntention()
    {
        try
        {
            m_Controller.SimpleCollectibleInventory.RemoveItem(SimpleCollectible.CratePoint);
            Instantiate(m_Controller.CratePrefab, transform.position + transform.forward + transform.up, Quaternion.identity);
        }
        catch (InventoryEmptyException<SimpleCollectible> e)
        {
            Debug.LogWarning(e);
        }
    }
}
