using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModifier : MonoBehaviour, IHealthTriggers
{
    public Health health;
    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }
}
