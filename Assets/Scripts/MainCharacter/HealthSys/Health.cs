using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using JetBrains.Annotations;

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
    public DeathRenderer deathRenderer;
    [CanBeNull]
    ClickSound cs;
    [SerializeField] AudioClip deathSound;

    [SerializeField]
    public GameObject scoringSystem;
    
    private volatile int m_LockCount;

    private class HealthLockGuard : HLockGuard
    {
        private readonly Health m_Health;
        
        internal HealthLockGuard(Health health)
        {
            m_Health = health;
            m_Health.InternalLock();
        }
        
        ~HealthLockGuard()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (m_Health)
            {
                m_Health.InternalUnLock();
            }
        }
    }
    
    public HLockGuard Lock()
    {
        return new HealthLockGuard(this);
    }

    public void TakeDamage(float damage)
    {
        if (IsInvincible()) return;
        if (cs)
        {
            cs.Click();
        }
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
        if (cs)
        {
            cs.Click();
        }
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
    
    public bool IsInvincible()
    {
        return m_LockCount > 0;
    }

    private void InternalLock()
    {
        Interlocked.Increment(ref m_LockCount);
    }

    private void InternalUnLock()
    {
        Interlocked.Decrement(ref m_LockCount);
    }
}


