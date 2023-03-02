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
        GameObject.Destroy(this.gameObject);
    }
}
