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
    private GameObject m_Player;
    [Range(0, 10)]
    public float attackDistance = 2f;
    public NavMeshAgent agent;
    [SerializeField][Range(0.5f, 3.0f)]
    private float attackCooldown = 1.5f;
    private bool canAttack = true;
    private float cachedSpeed;
    ClickSound cs;

    private void Start()
    {
        cs = GetComponent<ClickSound>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        FreezeRotation_XZ();
        animator.SetFloat("Speed", agent.velocity.magnitude);
        float distanceToPlayer = (transform.position - m_Player.transform.position).magnitude;
        if (distanceToPlayer <= attackDistance) Attack();
    }


    public void Attack()
    {
        if (canAttack)
        {
            cs.Click();
            animator.SetTrigger("Attack");
            StartCoroutine(CoolDown());
            FreezeOnCurrentState();
        }
    }

    private IEnumerator CoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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

    private void FreezeOnCurrentState()
    {
        canAttack = false;
        agent.isStopped = true;
    }

    public void UnFreeze()
    {
        canAttack = true;
        agent.isStopped = false;
    }
}
