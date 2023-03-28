using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static Unity.Mathematics.math;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class DBufferController : MonoBehaviour
{
    public VisualEffect attackVFX;
    private Animator animator;
    public Health health;
    public float moveSpeed = 0.5f;
    private Rigidbody rb;
    public float boundsSize = 10f;
    private float3 movementDirection;
    private GameObject m_Player;
    [Range(0, float.PositiveInfinity)]
    public float followDistance = 10f;
    [Range(0, float.PositiveInfinity)]
    public float attackDistance = 2f;
    
    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        health = GetComponent<Health>();
        health.OnDeath += OnDBufferDeath;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ChangeDirection());     
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        FreezeRotation_XZ();
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

    public void OnDBufferDeath()
    {
        //Do something
    }

    private IEnumerator ChangeDirection()
    {
        float randomTime1 = Random.Range(2.0f, 5.0f);
        float randomTime2 = Random.Range(1.4f, 3.8f);
        movementDirection = GetRandomDirection();
        yield return new WaitForSeconds(randomTime1);
        movementDirection = Vector3.zero;
        yield return new WaitForSeconds(randomTime2);
        StartCoroutine(ChangeDirection());
    }

    private float3 GetRandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        return normalize(float3(x, 0, z));
    }

    public float3 GetMovementDirection()
    {
        return movementDirection;
    }

    private void FixedUpdate()
    {
        // check distance
        float dist = length(m_Player.transform.position - transform.position);
        if (dist <= followDistance && dist > attackDistance)
        {
            movementDirection = normalize(m_Player.transform.position - transform.position);
        }
        else if (dist <= attackDistance)
        {
            movementDirection = Vector3.zero;
            Attack();
        }

        rb.velocity = movementDirection * moveSpeed;
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }


    private bool IsOutOfBounds()
    {
        return transform.position.x < -boundsSize || transform.position.x > boundsSize ||
               transform.position.z < -boundsSize || transform.position.z > boundsSize;
    }
}
