using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public event Action<int,int> OnHealthChange;
    public event Action OnDeath;
    private int currentHealth = 5;
    private int MaxHealth = 5;
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
}


