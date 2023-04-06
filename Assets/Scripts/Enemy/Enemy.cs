using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, I_AI_Trigger
{
    [SerializeField]
    private Health enemyHealth;
    //private EnemyAI ai;
    private NavMeshAgent agent;
    private float cachedSpeed;
    private float cachedAcceleration;
    private float cachedAngularSpeed;
    private Rigidbody rb;
    public bool canMove = true;
    public DungeonRoom attachedRoom;
    public bool isDying = false;

    [SerializeField]
    public GameObject scoringSystem;

    private void Start()
    {
        //TryGetComponent<EnemyAI>(out ai);
        TryGetComponent<NavMeshAgent>(out agent);
        rb = GetComponent<Rigidbody>();
        enemyHealth.OnDeath += OnEnemyDeath;
        if(agent)
        {
            cachedSpeed = agent.speed;
            cachedAcceleration = agent.acceleration;
            cachedAngularSpeed = agent.angularSpeed;
        }
        scoringSystem = GameObject.Find("ScoringSystem");
    }

    public void OnEnemyDeath() {
        EnemyAI ai;
        TryGetComponent<EnemyAI>(out ai);
        isDying = true;
        if(attachedRoom != null)
        attachedRoom.TryRemoveEnemy(this);
        //if (ai) ai.GetAttachedRoom().RemoveEnemy(ai);
        enemyHealth.deathRenderer.OnDeathRender();
        if (agent)
        {
            agent.isStopped = true;
        }
        scoringSystem.Trigger<IScoringSystemTriggers>(nameof(IScoringSystemTriggers.OnEnemyDeath));
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
