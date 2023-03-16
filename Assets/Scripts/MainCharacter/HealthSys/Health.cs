using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHealthTriggers : ITrigger
{
    void TakeDamage(int damage);
}

public class Health : MonoBehaviour, IHealthTriggers
{
    public event Action<int,int> OnHealthChange;
    public event Action OnDeath;
    [SerializeField]
    private int currentHealth = 5;
    private int MaxHealth = 5;
    [SerializeField]
    private HealthUI ui; 

    public void TakeDamage(int damage)
    {
        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
            Death();
        }
        else {
            currentHealth -= damage;
            OnHealthChange?.Invoke(-damage, currentHealth);
        }        
    }

    public void GainHealth(int healthGain) {
        if (currentHealth + healthGain >= MaxHealth)
        {
            currentHealth = MaxHealth;
            OnHealthChange?.Invoke(healthGain, currentHealth);
        }
        else {
            currentHealth += healthGain;
            OnHealthChange?.Invoke(healthGain, currentHealth);
        }
        
    }

    public void Death() {
        OnDeath?.Invoke();
    }

    private void Start()
    {
        if (gameObject.CompareTag("Player")) {
            ui.GenerateHearts(currentHealth);
        }
        
    }
}


