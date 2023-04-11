using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private bool isTargetable;
    private void OnEnable()
    {
        TryGetComponent(out agent);
        TryGetComponent(out rb);
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

    public void OnEnemyDeath() 
    {
        isDying = true;
        enemyHealth.deathRenderer.OnDeathRender();
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

    public async void FreezeOnCurrentState()
    {
        isTargetable = false;
        {
            using HLockGuard healthLock = enemyHealth.Lock();
            DeathRenderer rend = enemyHealth.deathRenderer;
            if (rend != null) enemyHealth.deathRenderer.ComponentsFreeze();
            DisableAgent();
            if (agent.isActiveAndEnabled) agent.isStopped = true;
            try
            {
                GetComponentInChildren<DBufferController>().FreezeOnCurrentState();
            }
            catch
            {
                //ignore
            }
            while (!isTargetable)
            {
                await Task.Yield();
            }
        }
    }

    public async void UnFreeze()
    {
        isTargetable = true;
        DeathRenderer rend = enemyHealth.deathRenderer;
        if(rend != null) enemyHealth.deathRenderer.EnableComponents();
        EnableAgent();
        if (agent.isActiveAndEnabled) agent.isStopped = false;
        try
        {
            await Task.Delay(2000);
            GetComponentInChildren<DBufferController>().UnFreeze();
        }
        catch
        {
            //ignore
        }
    }


    public void EnableAgent()
    {
        Debug.Assert(rb != null && agent != null);
        rb.isKinematic = true;
        agent.speed = cachedSpeed;
        agent.acceleration = cachedAcceleration;
        agent.angularSpeed = cachedAngularSpeed;     
    }
}
