using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, I_AI_Trigger
{
    [SerializeField]
    private Health enemyHealth;
    private EnemyAI ai;
    private NavMeshAgent agent;
    private float cachedSpeed;
    private float cachedAcceleration;
    private float cachedAngularSpeed;
    private Rigidbody rb;

    private void Start()
    {
        TryGetComponent<EnemyAI>(out ai);
        TryGetComponent<NavMeshAgent>(out agent);
        rb = GetComponent<Rigidbody>();
        enemyHealth.OnDeath += OnEnemyDeath;
        if(agent)
        {
            cachedSpeed = agent.speed;
            cachedAcceleration = agent.acceleration;
            cachedAngularSpeed = agent.angularSpeed;
        }

    }

    public void OnEnemyDeath() {
        EnemyAI ai;
        TryGetComponent<EnemyAI>(out ai);
        if (ai) ai.GetAttachedRoom().RemoveEnemy(ai);
        enemyHealth.deathRenderer.OnDeathRender();
    }

    public void DisableAgent()
    {
        agent.angularSpeed = 0;
        agent.speed = 0;
        agent.acceleration = 0;
    }

    public void EnableAgent()
    {
        rb.isKinematic = true;
        agent.speed = cachedSpeed;
        agent.acceleration = cachedAcceleration;
        agent.angularSpeed = cachedAngularSpeed;
    }
}
