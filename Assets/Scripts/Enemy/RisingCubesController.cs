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

    public async void SpawnGravityGrenades()
    {
        /*while (true)
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

    




}
