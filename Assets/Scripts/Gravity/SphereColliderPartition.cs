using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereColliderPartition : MonoBehaviour
{
    private SpatialPartitioning spatialPartitioning;
    private void Start()
    {
        // Get a reference to the SpatialPartitioning script
        spatialPartitioning = GetComponent<SpatialPartitioning>();
        // Register the collider with the SpatialPartitioning script
        spatialPartitioning.RegisterCollider(GetComponent<Collider>());
    }

    private void Update()
    {
        // Get the nearby colliders from the SpatialPartitioning script
        List<Collider> nearbyColliders = spatialPartitioning.GetNearbyColliders(transform.position, GetComponent<SphereCollider>().radius);

        // Check for collisions with the nearby colliders
        foreach (Collider collider in nearbyColliders)
        {
            if (collider != GetComponent<Collider>() && collider.bounds.Intersects(GetComponent<Collider>().bounds))
            {
                Debug.Log("Collision detected with " + collider.name);
            }
        }
    }
}
