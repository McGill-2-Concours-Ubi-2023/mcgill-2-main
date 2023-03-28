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

    private void Start()
    {
        TryGetComponent<EnemyAI>(out ai);
        TryGetComponent<NavMeshAgent>(out agent);
        enemyHealth.OnDeath += OnEnemyDeath; 
    }

    public void OnEnemyDeath() {
        EnemyAI ai;
        TryGetComponent<EnemyAI>(out ai);
        if (ai) ai.GetAttachedRoom().RemoveEnemy(ai);
        enemyHealth.deathRenderer.OnDeathRender();
    }

    public void DisableAgent()
    {
        //TODO
    }

    public void EnableAgent()
    {
        //TODO
    }
}
