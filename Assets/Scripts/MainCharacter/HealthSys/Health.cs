using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHealthTriggers : ITrigger
{
    void TakeDamage(float damage);
    void GainHealth(float health);
    void IncreaseMaxHealth(float amount);
}

public class Health : MonoBehaviour, IHealthTriggers, IGravityGrenadeHealthAdaptor
{
    public event Action<float, float> OnHealthChange;
    public event Action OnDeath;
    [SerializeField]
    public float currentHealth = 5;
    private float MaxHealth = 5;
    [SerializeField]
    private HealthUI ui;
    public bool invulnerable;
    public DeathRenderer deathRenderer;
    ClickSound cs;
    [SerializeField] AudioClip deathSound;

    [SerializeField]
    public GameObject scoringSystem;

    public void TakeDamage(float damage)
    {
        if (invulnerable) return;
        cs.Click();
        if (gameObject.CompareTag("Player"))
        {
            scoringSystem.Trigger<IScoringSystemTriggers, float>(nameof(IScoringSystemTriggers.OnDamageTaken), damage);
        }
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

    public void GainHealth(float healthGain) {
        if ((currentHealth + healthGain) >= MaxHealth)
        {
            float difference = MaxHealth - currentHealth;
            currentHealth = MaxHealth;
            OnHealthChange?.Invoke(difference, currentHealth);
        }
        else {
            currentHealth += healthGain;
            OnHealthChange?.Invoke(healthGain, currentHealth);
        }
        
    }

    public void Death() {
        cs.Click(deathSound);
        OnDeath?.Invoke();
    }

    private void Start()
    {
        if (gameObject.CompareTag("Player")) {
            ui.GenerateHearts((int)currentHealth);
        }
        cs = GetComponent<ClickSound>();
        scoringSystem = GameObject.Find("ScoringSystem");
    }
    public void IncreaseMaxHealth(float amount) {
        MaxHealth = MaxHealth += amount;
        GainHealth(1);
    }
}


