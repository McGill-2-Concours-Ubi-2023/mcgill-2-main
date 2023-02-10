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
        ApplyMassCompression();
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isActive)
        isInAtmosphere = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (isActive) 
        {
            agents.Where(agent => agent.name.Equals(other.name))
                .ToList()
                .ForEach(agent => ApplyGravity(agent.GetComponent<Rigidbody>()));
        }      
    }

    private void OnTriggerExit(Collider other)
    {
        if(isActive)
        isInAtmosphere = false;
    }

    public void ReleaseAgent(GameObject obj)
    {
        agents.Remove(obj);
    }

    public void ApplyMassCompression()
    {
        agents.ForEach(agent => agent.GetComponent<GravityAgent>().SetMassCompression(massCompression));
    }
}
