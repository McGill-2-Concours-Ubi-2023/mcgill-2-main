using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LazerPlayerCollider : MonoBehaviour
{
    private CinemachineCameraShake cameraShake;
    private Vibration vibration;
    private LazerBeamCollider parentCollider;
    // Start is called before the first frame update

    private void OnEnable()
    {
        parentCollider = GetComponentInParent<LazerBeamCollider>();
        cameraShake = parentCollider.cameraShake;
        vibration = parentCollider.vibration;
    }
    private async void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(1);
            if (!other.GetComponent<Health>().IsInvincible())
            vibration.SharpVibration();
            cameraShake.StandardCameraShake(1.0f, 1.0f, 1.0f, 0);
            await Task.Delay(500);
            cameraShake.StopCameraShake();
        }
    }
}
