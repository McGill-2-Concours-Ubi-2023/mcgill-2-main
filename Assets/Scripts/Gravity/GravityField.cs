using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class GravityField : MonoBehaviour
{
    [Range(0.1f, 9.81f)]
    public float gravity;
    public List<GameObject> agents;
    protected bool isInAtmosphere;
    protected bool isActive;
    [SerializeField][Range(1,10)]
    protected float massCompression;
    protected abstract void ApplyGravity(Rigidbody rb);

    private void Awake()
    {
        agents = new List<GameObject>();
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.GetComponent<GravityAgent>())
        {
            isInAtmosphere = true;
            agents.Add(other.gameObject);
        }
    }

    public void SetMassCompressionForce(float compressionForce)
    {
        massCompression = compressionForce;
    }

    public float GetMassCompressionForce()
    {
        return massCompression;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (isActive) 
        {
            var agent = other.GetComponent<GravityAgent>();
            if (!agents.Contains(other.gameObject) && agent && !agent.IsBound())
            {
                agents.Add(other.gameObject);
            }
            agents = agents.Where(agent => agent).ToList();
            agents.Where(agent => agent.name.Equals(other.name))
                .ToList()
                .ForEach(agent => ApplyGravity(agent.GetComponent<Rigidbody>()));
        }      
    }

    private void OnTriggerExit(Collider other)
    {
        if (isActive)
        {
            isInAtmosphere = false;
            ReleaseAgent(other.gameObject);
        }
    }

    public void ReleaseAgent(GameObject obj)
    {
        agents.Remove(obj);
    }

    private void OnDestroy()
    {
        agents.ForEach(agent =>
        {
            if(agent != null)
            agent.GetComponent<GravityAgent>().Release();
        });
    }
}