using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RisingCubesController : MonoBehaviour
{
    // Start is called before the first frame update
    private CinemachineCameraShake cameraShake;
    public GameObject gravityGrenadePickup;

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
            agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    public void SpawnGravityGrenades()
    {

    }




}
