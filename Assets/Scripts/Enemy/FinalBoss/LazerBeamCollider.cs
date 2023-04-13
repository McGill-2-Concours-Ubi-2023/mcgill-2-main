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
    public List<Collider> obstacles;
    public CinemachineCameraShake cameraShake;
    public Vibration vibration;
    private FinalBossController bossController;
    private LayerMask obstacleMask;

    private void OnEnable()
    {
        if (obstacles == null) obstacles = new List<Collider>();
        bossController = FindObjectOfType<FinalBossController>();
        cameraShake = bossController.cameraShake;
        vibration = bossController.vibration;
        obstacleMask = Destructible.desctructibleMask;
    }

    public void ActivateCollider()
    {
        GetComponent<AudioSource>().Play();
        StartCoroutine(OnActivationWait());      
    }

    IEnumerator OnActivationWait()
    {
        _collider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        playerDamageCollider.enabled = true;
    }

    private async void OnTriggerEnter(Collider other)
    {
        bool isMask = other.gameObject.layer == obstacleMask;
        if (!obstacles.Contains(other) && isMask)
        {
            obstacles.Add(other);
        }
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health.IsInvincible()) obstacles.Add(other);
            await Task.Delay(200);
            obstacles.Remove(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        bool isMask = other.gameObject.layer == obstacleMask;
        if (!obstacles.Contains(other) && isMask)
        {
            obstacles.Add(other);
        }
    }

    private void Update()
    {
        UpdateLaser(obstacles);
    }

    private void UpdateLaser(List<Collider> colliders)
    {
        Collider closestCollider = FindClosestCollider(colliders);
        if(closestCollider != null)
        {
            beamVFX.SetVector3("ObstaclePosition", closestCollider.transform.position);
            beamVFX.SetFloat("ColliderSize", closestCollider.transform.lossyScale.magnitude);
            Vector3 diff = closestCollider.transform.position - colliderEnd.transform.position;
            Vector3 diff2 = closestCollider.transform.position - beamEnd.transform.position;
            beamVFX.SetFloat("ObstacleDistance", diff2.magnitude);
            transform.position = transform.parent.position - transform.forward * diff.magnitude;
        }       
        if(colliders.Count == 0)
        {
            transform.position = transform.parent.position;
            beamVFX.SetFloat("ObstacleDistance", 0);
        }
    }

    private Collider FindClosestCollider(List<Collider> colliders)
    {
        float closestDistance = float.MaxValue;
        Collider closestCollider = null;
        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = collider;
            }
        }
        return closestCollider;
    }

    private async void OnTriggerExit(Collider other)
    {
        try
        {
            obstacles.Remove(other);
        }
        catch
        {
            Debug.Log("Collider not found!");
        }
        playerDamageCollider.enabled = false;
        await Task.Delay(100);
        if(playerDamageCollider != null)
        {
            playerDamageCollider.enabled = true;
        }
    }        
}
