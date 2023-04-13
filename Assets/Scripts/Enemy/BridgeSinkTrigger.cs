using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BridgeSinkTrigger : MonoBehaviour
{
    public Animator animator;
    public Collider wall;
    // Start is called before the first frame update
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wall.enabled = true;
            animator.enabled = true;
            FindObjectOfType<FinalBossController>().cameraShake.StandardCameraShake(3.0f, 1.5f, 0);
            await Task.Delay(TimeSpan.FromSeconds(6));
            FindObjectOfType<FinalBossController>().cameraShake.StopCameraShake();
            GetComponent<Collider>().enabled = false;
        }
    }
}
