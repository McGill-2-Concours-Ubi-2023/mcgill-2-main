using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{
    Rumbler rumbler;
    Health health; 

    private void Awake()
    {
        rumbler = GameObject.Find("gamepadVib").GetComponent<Rumbler>();
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        health.OnHealthChange += HealthVibrate;
        health.OnDeath += SharpVibration;
    }
    public void SoftVibration() {
        rumbler.TriggerVibration(1, 0.1f, 0.1f);
    }

    public void SharpVibration() {
        rumbler.TriggerVibration(0.5f, 0.9f, 0.9f);
    }

    public void HealthVibrate(float change, float current) {
        if (change > 0)
        {
            SoftVibration();
            Debug.Log("softvibrate");
        }
        else if (change < 0) {
            SharpVibration();
            Debug.Log("sharpvibrate");
        }

        
    }
}
