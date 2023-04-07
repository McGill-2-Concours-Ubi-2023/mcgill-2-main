using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class LazerBeamCollider : MonoBehaviour
{
    private CapsuleCollider _collider;
    public Transform beamEnd;
    public VisualEffect beamVFX;
    public List<GameObject> obstacles;
    private GameObject currentObstacle;
    private bool hasCollided;

    private void OnEnable()
    {       
        _collider = GetComponent<CapsuleCollider>();
        _collider.enabled = false;
        if (obstacles == null) obstacles = new List<GameObject>();
    }

    public void ActivateCollider()
    {
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!obstacles.Contains(other.gameObject))
        {
            obstacles.Add(other.gameObject);
        }

        if (other.gameObject.layer == Destructible.desctructibleMask)
        {
            beamVFX.SetVector3("ObstaclePosition", other.transform.position);
            beamVFX.SetFloat("ColliderSize", other.bounds.size.magnitude);
            Vector3 diff = other.transform.position - beamEnd.transform.position;
            transform.position = transform.parent.position - transform.forward * diff.magnitude;
            hasCollided = true;
        }       

        if(obstacles.Count > 1)
        {
            Vector3 average = new Vector3(0,0,0);
            foreach(GameObject obstacle in obstacles)
            {
                average += obstacle.transform.position;
            }
            Vector3 midPoint = average / obstacles.Count;
            beamVFX.SetVector3("ObstaclePosition", midPoint);

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        obstacles.Remove(other.gameObject);
        if (other.gameObject.layer == Destructible.desctructibleMask && obstacles.Count == 0)
        {
            transform.position = transform.parent.position;
        }
    }
}
