using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAgent : MonoBehaviour
{
    private float massCompression;
    public GravityField currentField;
    private bool isBound = false;
    private bool isVanishing = false;
    private Animator animator;

    public IEnumerator OnWaitDestroy(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (isBound)
        {
            animator = GetComponent<Animator>();
            animator.speed *= massCompression;
            animator.SetTrigger("Despawn");
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

    //============SHIT CODE :DEPRECATED============= 
    /*private IEnumerator Disappear()
    {
        isVanishing = true;
        massCompression = currentField.GetMassCompressionForce();
        bool isVisible = true;
        float startTime = Time.time;

        while (isVisible && currentField != null) 
        {
            // Calculate the time elapsed since the start of the interpolation
            float elapsedTime = Time.time - startTime;
            // Calculate the new scale using an exponential decay function
            transform.localScale *= Mathf.Exp(-elapsedTime*massCompression/100);
            Vector3 deltaD = currentField.transform.position - transform.position;
            transform.position += deltaD.normalized * massCompression * Time.deltaTime;
            isVisible = transform.localScale.x > 0.5f && transform.localScale.y > 0.5f && transform.localScale.z > 0.5f;
            yield return 0;
        }
        Destroy(this.gameObject);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("DestructionBounds") 
            && gameObject.layer == LayerMask.NameToLayer("Destructible"))
        {
            BindField(other.GetComponentInParent<GravityField>());
            isBound = true;
            GravitationalGrenade grenade = other.gameObject.GetComponentInParent<GravitationalGrenade>();
            StartCoroutine(OnWaitDestroy(grenade.GetDestructionTimer()));       
        }
    }

    private void BindField(GravityField field)
    {
        currentField = field;
        massCompression = currentField.GetMassCompressionForce();
        foreach (var effectiveField in FindObjectsOfType<GravityField>())
        {
            if (effectiveField != currentField) effectiveField.ReleaseAgent(this.gameObject);
        }
    }

    public void Release()
    {
        isBound = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("DestructionBounds") 
            && gameObject.layer == LayerMask.NameToLayer("Destructible"))
        {
            if (other.GetComponentInParent<GravityField>() == currentField)
            {
                isBound = false;
                if(!isVanishing)
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
