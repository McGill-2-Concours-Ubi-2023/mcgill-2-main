using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModifier : MonoBehaviour, IHealthTriggers
{
    public Health health;

    public void GainHealth(int health)
    {
        throw new System.NotImplementedException();
    }

    public void IncreaseMaxHealth(int amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }
}
