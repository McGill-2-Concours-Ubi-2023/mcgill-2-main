using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravityField : MonoBehaviour
{
    [SerializeField]
    private GameObject destructionBounds;
    [Range(0.1f, 9.81f)]
    public float gravity;
    public List<GameObject> agents;
    protected bool isActive;
    protected int layerMask;
    [SerializeField]
    [Range(0.5f, 3.0f)]
    protected float massCompression = 1.0f;
    private HashSet<Rigidbody> cachedRigidbodies = new HashSet<Rigidbody>();

    
    protected abstract void ApplyGravity(Rigidbody rb);
    protected abstract void DetectCollision();

    private void Awake()
    {
        if(agents == null)
        agents = new List<GameObject>();
    }

    public int GetLayerMask()
    {
        return layerMask;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        destructionBounds.SetActive(active);
    }

    public bool IsActive()
    {
        return isActive;
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
                if (rb.isKinematic) rb.isKinematic = false;
                if (rb != null)
                {
                    agents.Add(other.gameObject);
                    cachedRigidbodies.Add(rb);
                    agent.BindField(this);
                }
            }

            if(other.tag == "Player")
            {
                other.GetComponent<Animator>().SetBool("IsFloating", true);
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
        if (isActive)
        {
            ReleaseAgent(other.gameObject);  
        }
    }

    public void ReleaseAgent(GameObject obj)
    {
        if (obj.tag == "Player")
        {
            obj.GetComponent<Animator>().SetBool("IsFloating", false);
        }
        agents.Remove(obj);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            cachedRigidbodies.Remove(rb);
        }
    }

    private void OnDestroy()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if(player)player.GetComponent<Animator>().SetBool("IsFloating", false);
        cachedRigidbodies.Clear();
    }
}
