using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static Unity.Mathematics.math;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class DBufferController : MonoBehaviour
{
    public VisualEffect attackVFX;
    private Animator animator;
    private float3 movementDirection;
    private GameObject m_Player;
    [Range(0, float.PositiveInfinity)]
    public float attackDistance = 2f;
    public NavMeshAgent agent;
    
    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Debug.Log(agent.velocity);
        FreezeRotation_XZ();
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    //triggered via animation event
    public void OnAttackSwing()
    {
        attackVFX.SendEvent("OnSwingStart");
    }

    //triggered via animation event
    public void OnSlash()
    {        
        attackVFX.SendEvent("OnSwingStop");
        attackVFX.SendEvent("OnSlash");
    }

    private void FreezeRotation_XZ()
    {
        transform.rotation = new Quaternion(0, transform.rotation.y, 0,transform.rotation.w);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == m_Player) //if arm hits the player
        {           
            //Unlucky, get better and dodge, stop blaming the developers for your lack of skills
            m_Player.Trigger<IHealthTriggers, int>(nameof(IHealthTriggers.TakeDamage), 1);
        }
    }
}
