using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LazerPlayerCollider : MonoBehaviour
{
    private CinemachineCameraShake cameraShake;
    private Vibration vibration;
    private LazerBeamCollider parentCollider;
    private MainCharacterController player;
    // Start is called before the first frame update

    private void OnEnable()
    {
        parentCollider = GetComponentInParent<LazerBeamCollider>();
        cameraShake = parentCollider.cameraShake;
        vibration = parentCollider.vibration;
        player = FindObjectOfType<MainCharacterController>();
    }
    private async void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = player.GetComponent<Health>();
            if (!playerHealth.IsInvincible())
            {
                playerHealth.TakeDamage(1);
                vibration.SharpVibration();
                cameraShake.StandardCameraShake(4.0f, 2.0f, 0);
                await Task.Delay(500);
                cameraShake.StopCameraShake();
            }           
        }
    }
}
