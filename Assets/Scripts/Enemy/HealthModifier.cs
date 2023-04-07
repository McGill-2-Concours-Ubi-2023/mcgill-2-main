using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModifier : MonoBehaviour, IHealthTriggers, IGravityGrenadeHealthAdaptor
{
    public Health health;

    public void GainHealth(float health)
    {
        throw new System.NotImplementedException();
    }

    public void IncreaseMaxHealth(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void SetInvincible(bool invincible)
    {
        health.SetInvincible(invincible);
    }

    public void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
    }
}
