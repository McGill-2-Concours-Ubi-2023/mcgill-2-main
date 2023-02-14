using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSysTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Health health; 
    void Start()
    {
        health.OnHealthChange += Testing_OnHealthChange;
        health.OnDeath += Testing_OnDeath;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        health.TakeDamage(3); 
    }

    public void Testing_OnHealthChange(int change, int health) {
        Debug.Log(change + " " + health);
    }

    public void Testing_OnDeath() {
        Debug.Log("Ah I'm ded :C");
    }
}
