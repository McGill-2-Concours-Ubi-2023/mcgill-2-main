using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravityAgent : MonoBehaviour
{
    private float massCompression;
    public GravityField currentField;
    [SerializeField]
    private bool isBound = false;
    private Animator animator;

    public IEnumerator OnWaitDestroy(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (isBound)
        {
            Animator animator;
            TryGetComponent<Animator>(out animator);
            if (animator)
            {
                animator.speed *= massCompression;
                animator.SetTrigger("Despawn");
            }          
        }
    }

    public bool IsBound()
    {
        return isBound;
    }

    public void OnSelfDestroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("DestructionBounds") 
            && gameObject.layer == LayerMask.NameToLayer("Destructible"))
        {
            if (currentField.IsActive()) 
            {
                isBound = true;
            }
            GravitationalGrenade grenade = other.gameObject.GetComponentInParent<GravitationalGrenade>();
            StartCoroutine(OnWaitDestroy(grenade.GetDestructionTimer()));       
        }
    }

    public void BindField(GravityField field)
    {
        currentField = field;
        gameObject.Trigger<I_AI_Trigger>("DisableAgent");
        massCompression = currentField.GetMassCompressionForce();
        foreach (var effectiveField in FindObjectsOfType<GravityField>())
        {

            if (effectiveField != currentField) effectiveField.ReleaseAgent(this.gameObject);
        }
    }

    public void Release()
    {
        gameObject.Trigger<I_AI_Trigger>("EnableAgent");
        isBound = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("DestructionBounds") 
            && gameObject.layer == LayerMask.NameToLayer("Destructible"))
        {
            if (other.GetComponentInParent<GravityField>() == currentField)
            {
                Release();
                StopAllCoroutines();
            }
        }
    }

    private void OnDestroy()
    {
        foreach(var field in FindObjectsOfType<GravityField>())
        {
            if (field.agents.Contains(this.gameObject)) field.ReleaseAgent(this.gameObject);
        }
    }
}
