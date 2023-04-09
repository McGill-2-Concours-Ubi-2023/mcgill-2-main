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

    public async void ActivateCollider()
    {
        int obstacleLayerMask = 1 << Destructible.desctructibleMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100, obstacleLayerMask))
        {
            Vector3 diff = hit.transform.position - beamEnd.transform.position;
            transform.position = transform.parent.position - transform.forward * diff.magnitude;
            await Task.Delay(100);          
        }
        _collider.enabled = true;
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
            beamVFX.SetVector3("ObstaclePosition", other.transform.position);
            beamVFX.SetFloat("ColliderSize", other.bounds.size.magnitude);
            Vector3 diff = other.transform.position - colliderEnd.transform.position;
            Vector3 diff2 = other.transform.position - beamEnd.transform.position;
            beamVFX.SetFloat("ObstacleDistance", diff2.magnitude);
            transform.position = transform.parent.position - transform.forward * diff.magnitude;
        }
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
