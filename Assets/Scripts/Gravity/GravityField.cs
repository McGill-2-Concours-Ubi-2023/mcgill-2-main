using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravityField : MonoBehaviour
{
    [Range(0.1f, 9.81f)]
    public float gravity;
    public List<GameObject> agents;
    protected bool isActive;
    protected int layerMask;
    [SerializeField]
    [Range(1, 20)]
    protected float massCompression;
    private HashSet<Rigidbody> cachedRigidbodies = new HashSet<Rigidbody>();

    protected abstract void ApplyGravity(Rigidbody rb);
    protected abstract void DetectCollision();

    private void Awake()
    {
        agents = new List<GameObject>();
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void SetMassCompressionForce(float compressionForce)
    {
        massCompression = compressionForce;
    }

    public float GetMassCompressionForce()
    {
        return massCompression;
    }

    //collision condition set in collision matrix go to Edit > Project settings > Layer collision matrix
    protected void ProcessCollision(Collider other)
    {
        if (isActive)
        {
            var agent = other.GetComponent<GravityAgent>();
            if (!agents.Contains(other.gameObject) && agent && !agent.IsBound())
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    agents.Add(other.gameObject);
                    cachedRigidbodies.Add(rb);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateLayerMask();
        DetectCollision();
        foreach (Rigidbody rb in cachedRigidbodies)
        {
            if (rb != null)
            {
                ApplyGravity(rb);
            }
        }
    }

    private void UpdateLayerMask()
    {
        int layerMask1 = 1 << LayerMask.NameToLayer("Destructible"); // set the first layer mask 1"
        int layerMask2 = 1 << LayerMask.NameToLayer("Player"); // set the second layer mask 
        layerMask = layerMask1 | layerMask2; // combine the layer masks
    }

    private void OnTriggerExit(Collider other)
    {
        if (isActive && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ReleaseAgent(other.gameObject);
        }
    }

    public void ReleaseAgent(GameObject obj)
    {
        agents.Remove(obj);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            cachedRigidbodies.Remove(rb);
        }
    }

    private void OnDestroy()
    {
        foreach (Rigidbody rb in cachedRigidbodies)
        {
            if (rb != null)
            {
                rb.gameObject.GetComponent<GravityAgent>().Release();
            }
        }
        cachedRigidbodies.Clear();
    }
}
