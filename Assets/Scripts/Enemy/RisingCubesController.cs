using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RisingCubesController : MonoBehaviour
{
    // Start is called before the first frame update
    private CinemachineCameraShake cameraShake;
    public GameObject gravityGrenadePickup;
    public List<Transform> spawnPoints;
    private bool cubesActive;
    private void Awake()
    {
        cameraShake = FindObjectOfType<FinalBossController>().cameraShake;
    }

    public async void OnCubeDescendCameraShake()
    {
        cameraShake.StandardCameraShake(2.0f, 1.0f, 0);
        await Task.Delay(500);
        cameraShake.StopCameraShake();
    }

    public async void OnCubeAscendCameraShake()
    {
        cameraShake.StandardCameraShake(3.0f, 2.0f, 0);
        await Task.Delay(500);
        cameraShake.StopCameraShake();
    }

    public void UpdateCubes()
    {
        var agents = GetComponentsInChildren<GravityAgent>();
        foreach(var agent in agents)
        {
            Rigidbody rb = agent.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void FreeCubes()
    {
        GetComponent<Animator>().enabled = false;
        
    }

    public async void SpawnGravityGrenades()
    {
        cubesActive = true;
        while (cubesActive)
        {
            int randIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector3 spawnPosition = spawnPoints[randIndex].position;
            GameObject pickup = Instantiate(gravityGrenadePickup);
            pickup.transform.position = spawnPosition;
            await Task.Delay(TimeSpan.FromSeconds(5));                     
        }
    }

    public void ReleaseCubes()
    {
        cubesActive = false;
        var agents = GetComponentsInChildren<GravityAgent>();
        foreach(var agent in agents)
        {
            Rigidbody rb = agent.GetComponent<Rigidbody>();
            //rb.useGravity = false;
            rb.GetComponent<Collider>().enabled = false;
        }
    }

    




}
