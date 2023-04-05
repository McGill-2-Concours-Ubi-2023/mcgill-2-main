using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Vibration : MonoBehaviour, IVibrationTrigger
{
    [SerializeField]
    Rumbler rumbler;
    [SerializeField]
    Health health;

    private void Start()
    {
        rumbler = gameObject.GetComponent<Rumbler>();
        if (SceneManager.GetActiveScene().name != "Game")
        {
            StopVibration();
        }
        else {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            health.OnHealthChange += HealthVibrate;
            health.OnDeath += SharpVibration;
        }

        
    }

    private void Update()
    {
    }
    public void SoftVibration() {
        rumbler.TriggerVibration(0.1f, 0.1f, 0.1f);
    }

    public void SharpVibration() {
        rumbler.TriggerVibration(0.5f, 0.9f, 0.9f);
    }

    public void StopVibration()
    {
        rumbler.TriggerVibration(0.5f, 0f, 0f);
    }

    public void HealthVibrate(float change, float current) {
        if (change > 0)
        {
            SoftVibration();
        }
        else if (change < 0) {
            SharpVibration();
        }
    }

    public void StartRumbling() {// rumbles softly until stopRumbling  is called 
        rumbler.StartVibration(0.1f, 0.1f);
        Debug.Log("startVibration");
    }

    public void StopRumbling() {
        rumbler.StopVibration();
        Debug.Log("stopVibration");
    }

}
