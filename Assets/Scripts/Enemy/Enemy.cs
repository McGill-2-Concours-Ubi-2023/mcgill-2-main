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

    [SerializeField]
    private Material hurtMaterial;
    [SerializeField]
    private Material originalMaterial; 
    [SerializeField]
    private float duration;
    [SerializeField]
    private int numOfFlashes;
    [SerializeField]
    private SkinnedMeshRenderer mr;
    [SerializeField]
    private MeshRenderer mr2;
    private void Start()
    {
        //TryGetComponent<EnemyAI>(out ai);
        TryGetComponent<NavMeshAgent>(out agent);
        rb = GetComponent<Rigidbody>();
        enemyHealth.OnDeath += OnEnemyDeath;
        if (mr != null)
        {
            originalMaterial = mr.material;
        }
        else originalMaterial = mr2.material;
        enemyHealth.OnHealthChange += Flash;
        
        if(agent)
        {
            cachedSpeed = agent.speed;
            cachedAcceleration = agent.acceleration;
            cachedAngularSpeed = agent.angularSpeed;
        }
        scoringSystem = GameObject.Find("ScoringSystem");
    }

    public void OnEnemyDeath() {
        isDying = true;
        enemyHealth.deathRenderer.OnDeathRender();
        if (agent)
        {
            agent.isStopped = true;
        }
        scoringSystem.Trigger<IScoringSystemTriggers>(nameof(IScoringSystemTriggers.OnEnemyDeath));
    }

    public void Flash(float change, float current) {
        if (change >= 0) return;
        StartCoroutine(Flash());
    }

    IEnumerator Flash() {
        int i = 0;
        float timer = 0f;
        while (i < numOfFlashes)
        {
            if (timer < duration/2)
            {
                if (isDying) yield break;
                if (mr != null)
                {
                    mr.material = hurtMaterial;
                }
                else mr2.material = hurtMaterial;
            }
            else if (timer < duration)
            {
                if (isDying) yield break;
                if (mr != null)
                {
                    mr.material = originalMaterial;
                }
                else mr2.material = originalMaterial;
            }
            else
            {
                timer = 0f;
                i++;
            }
            timer += Time.deltaTime;
            yield return null;
        }
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
