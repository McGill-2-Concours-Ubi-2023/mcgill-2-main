using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSysTest : MonoBehaviour
{
    public Health health; 
    void Start()
    {
        health.OnHealthChange += Testing_OnHealthChange;
        health.OnDeath += Testing_OnDeath;
    }


    private void OnMouseDown()
    {
        health.TakeDamage(3); 
    }

    public void Testing_OnHealthChange(float change, float health) {
        Debug.Log(change + " " + health);
    }

    public void Testing_OnDeath() {
        Debug.Log("Ah I'm ded :C");
    }
}
