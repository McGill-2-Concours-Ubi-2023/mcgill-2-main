using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravityField : MonoBehaviour
{
    [Range(0.1f, 9.81f)]
    public float gravity;
    public List<GameObject> agents;
    protected bool isActive;
    [SerializeField]
    [Range(1, 20)]
    protected float massCompression;
    private HashSet<Rigidbody> cachedRigidbodies = new HashSet<Rigidbody>();

    protected abstract void ApplyGravity(Rigidbody rb);

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
    private void OnTriggerStay(Collider other)
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
        foreach (Rigidbody rb in cachedRigidbodies)
        {
            if (rb != null)
            {
                ApplyGravity(rb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isActive)
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
