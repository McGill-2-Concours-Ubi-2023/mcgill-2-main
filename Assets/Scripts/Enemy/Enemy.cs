using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Health enemyHealth;

    private void Start()
    {
        enemyHealth.OnDeath += OnEnemyDeath; 
    }

    public void OnEnemyDeath() {
        EnemyAI ai;
        TryGetComponent<EnemyAI>(out ai);
        if (ai) ai.GetAttachedRoom().RemoveEnemy(ai);
        GameObject.Destroy(this.gameObject);
    }
}
