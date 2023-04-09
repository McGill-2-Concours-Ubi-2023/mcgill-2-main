using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class LazerBeamCollider : MonoBehaviour
{
    public Collider _collider;
    public Transform beamEnd;
    public Transform colliderEnd;
    public VisualEffect beamVFX;
    public List<GameObject> obstacles;
    private bool hasCollided;

    private void OnEnable()
    {       
        if (obstacles == null) obstacles = new List<GameObject>();
    }

    public  void ActivateCollider()
    {
        int obstacleLayerMask = 1 << Destructible.desctructibleMask;
        OnSpawnDetectCollision(obstacleLayerMask);
    }

    private async void OnSpawnDetectCollision(int layerMask)
    {
        // Cast a ray from the current position in the forward direction
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        GameObject lastHitObject = null; // Variable to store the last hit object

        int maxHits = 10; ;
        int hits = 0;
        while (hits < maxHits)
        {
            if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerMask))
            {
                // Store the current hit object as the last hit object
                lastHitObject = hit.collider.gameObject;

                // Move the origin of the ray to the point of collision to continue casting
                origin = hit.point + direction * 0.001f;

                hits++;
            }
            else
            {
                break;
            }
            if(lastHitObject != null)
            {
                Vector3 diff = hit.transform.position - beamEnd.transform.position;
                transform.position = transform.parent.position - transform.forward * diff.magnitude;
                beamVFX.SetVector3("ObstaclePosition", hit.transform.position);
                beamVFX.SetFloat("ColliderSize", hit.collider.bounds.size.magnitude);
                Vector3 diff2 = hit.transform.position - beamEnd.transform.position;
                beamVFX.SetFloat("ObstacleDistance", diff2.magnitude);
                await Task.Delay(500);
            }
            _collider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!obstacles.Contains(other.gameObject))
        {
            obstacles.Add(other.gameObject);
        }

        if (other.gameObject.layer == Destructible.desctructibleMask && !hasCollided)
        {
            hasCollided = true;
        }       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(1);
        }
        if (!obstacles.Contains(other.gameObject))
        {
            obstacles.Add(other.gameObject);
        }
        if (other.gameObject.layer == Destructible.desctructibleMask && hasCollided)
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

    private void OnTriggerExit(Collider other)
    {
        // Start the coroutine to execute OnTriggerExit logic on the next frame
        obstacles.Remove(other.gameObject);
        hasCollided = false;
        StartCoroutine(OnTriggerExitCoroutine(other));
    }
    private IEnumerator OnTriggerExitCoroutine(Collider other)
    {
        // Might want to check for other colliders first
        yield return new WaitForSeconds(0.1f);
        if (other.gameObject.layer == Destructible.desctructibleMask && obstacles.Count == 0 && !hasCollided)
        {
            transform.position = transform.parent.position;
            beamVFX.SetFloat("ObstacleDistance", 0);
        }
    }

}
