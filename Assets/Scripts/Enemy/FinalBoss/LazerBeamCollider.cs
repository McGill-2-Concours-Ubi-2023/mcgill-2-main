using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class LazerBeamCollider : MonoBehaviour
{
    public Collider _collider;
    public Collider playerDamageCollider;
    public Transform beamEnd;
    public Transform colliderEnd;
    public VisualEffect beamVFX;
    public List<GameObject> obstacles;
    private bool hasCollided;
    public CinemachineCameraShake cameraShake;
    public Vibration vibration;
    private FinalBossController bossController;
    private GameObject currentObstacle;

    private void OnEnable()
    {       
        if (obstacles == null) obstacles = new List<GameObject>();
        bossController = FindObjectOfType<FinalBossController>();
        cameraShake = bossController.cameraShake;
        vibration = bossController.vibration;
    }

    public async void ActivateCollider()
    {
        GetComponent<AudioSource>().Play();
        int obstacleLayerMask = 1 << Destructible.desctructibleMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100, obstacleLayerMask))
        {
            Vector3 diff = hit.transform.position - colliderEnd.transform.position;
            transform.position = transform.parent.position - transform.forward * diff.magnitude;
            beamVFX.SetVector3("ObstaclePosition", hit.transform.position);
            beamVFX.SetFloat("ColliderSize", hit.collider.bounds.size.magnitude);
            Vector3 diff2 = hit.transform.position - beamEnd.transform.position;
            beamVFX.SetFloat("ObstacleDistance", diff2.magnitude);
        }
        await Task.Delay(100);
        _collider.enabled = true;
        playerDamageCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!obstacles.Contains(other.gameObject))
        {
            obstacles.Add(other.gameObject);
        }

        if (other.gameObject.layer == Destructible.desctructibleMask)
        {
            currentObstacle = other.gameObject;         
            hasCollided = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!obstacles.Contains(other.gameObject))
        {
            obstacles.Add(other.gameObject);
        }
        if (other.gameObject == currentObstacle && hasCollided)
        {
            UpdateLazer(other);
        }
    }

    private void UpdateLazer(Collider other)
    {
        beamVFX.SetVector3("ObstaclePosition", other.transform.position);
        beamVFX.SetFloat("ColliderSize", other.bounds.size.magnitude);
        Vector3 diff = other.transform.position - colliderEnd.transform.position;
        Vector3 diff2 = other.transform.position - beamEnd.transform.position;
        beamVFX.SetFloat("ObstacleDistance", diff2.magnitude);
        transform.position = transform.parent.position - transform.forward * diff.magnitude;
    }

    private async void OnTriggerExit(Collider other)
    {
        if (isActiveAndEnabled)
        {
            obstacles.Remove(other.gameObject);
            // Start the coroutine to execute OnTriggerExit logic on the next frame       
            if (other.gameObject.layer == Destructible.desctructibleMask && obstacles.Count == 0)
            {
                hasCollided = false;
                await Task.Delay(100);
                try
                {
                    transform.position = transform.parent.position;
                    beamVFX.SetFloat("ObstacleDistance", 0);
                }
                catch
                {
                    //Ignore
                }           
            }
        }       
    }        
}
