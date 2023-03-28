using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSeeingController : MonoBehaviour
{
    public Health health;
    private void Awake()
    {
        health.OnDeath += OnAllSeeingDeath;
    }

    // Update is called once per frame

    public void OnAllSeeingDeath()
    {
        health.deathRenderer.OnDeathRender();
        enabled = false;
    }
}
