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
        /*while (cubesActive)
        {
            int randIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            try
            {
                Vector3 spawnPosition = spawnPoints[randIndex].position;
                GameObject pickup = Instantiate(gravityGrenadePickup);
                pickup.transform.position = spawnPosition;
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            catch
            {
                Debug.Log("Transform already disposed!");
            }
           
        }*/
    }

    public void ReleaseCubes()
    {
        cubesActive = false;
        Debug.Log("RISING");
        var agents = GetComponentsInChildren<GravityAgent>();
        foreach(var agent in agents)
        {
            int randIndex = UnityEngine.Random.Range(0, 2);
            Rigidbody rb = agent.GetComponent<Rigidbody>();
            if (randIndex == 0) rb.velocity = (new Vector3(1, 1, 0) * 100);
            if (randIndex == 1) rb.velocity = (new Vector3(-1, -1, 0) * 100);
        }
    }

    




}
