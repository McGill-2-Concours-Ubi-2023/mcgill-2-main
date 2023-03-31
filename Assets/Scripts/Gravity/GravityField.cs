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
    protected int fieldMask;
    [SerializeField]
    [Range(0.5f, 3.0f)]
    protected float massCompression = 1.0f;
    protected List<Rigidbody> cachedRigidbodies;

    protected abstract void ApplyGravity(Rigidbody rb);
   // protected abstract void ApplyGravityJob();
   // protected abstract void DetectCollision();

    public void SetActive(bool active)
    {
        if (agents == null) agents = new List<GameObject>();
        if (cachedRigidbodies == null) cachedRigidbodies = new List<Rigidbody>();
        fieldMask = PlayerAgent.playerMask;//add masks to ignore the field's rotation correction
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
    public void ProcessCollision(Collider other)
    {
        if (isActive)
        {
            var agent = other.GetComponent<GravityAgent>();
            if (!agents.Contains(other.gameObject) && agent)
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

    private void OnTriggerStay(Collider other)
    {
        ProcessCollision(other);
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
        ReleaseAgent(other.gameObject);  
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
        foreach(var rb in cachedRigidbodies)
        {
            if(rb != null)
            {
                rb.GetComponent<GravityAgent>().Release();
            }
        }
        GameObject.Find("MainCharacter")
            .GetComponent<Animator>()
            .SetBool("IsFloating", false);
        cachedRigidbodies.Clear();
    }
}
