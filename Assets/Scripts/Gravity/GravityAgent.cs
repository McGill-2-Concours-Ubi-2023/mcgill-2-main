using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAgent : MonoBehaviour
{
    private float massCompression;
    public GravityField currentField;
    private bool isBound = false;
    private bool isVanishing = false;

    public IEnumerator OnWaitDestroy(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (isBound)
        {
            StartCoroutine(Disappear(0)); //spin the object for 1 second
        }
    }

    public bool IsBound()
    {
        return isBound;
    }

    private IEnumerator Disappear(float counter)
    {
        isVanishing = true;
        massCompression = currentField.GetMassCompressionForce();
        while(transform.localScale.magnitude > 0.25f) 
        {
            //Exponentially decrease the scale
            var decreaseFactor = Mathf.Exp(counter * massCompression * 2) * Time.deltaTime;
            //Update the scale
            transform.localScale -= transform.localScale * decreaseFactor;
            Vector3 deltaD = currentField.transform.position - transform.position;
            transform.position += deltaD.normalized * massCompression * Time.deltaTime;
            counter += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("DestructionBounds"))
        {
            BindField(other.GetComponentInParent<GravityField>());
            isBound = true;
            StartCoroutine(OnWaitDestroy(other.gameObject.GetComponentInParent<GravitationalGrenade>()
                .GetDestructionTimer()));       
        }
    }

    private void BindField(GravityField field)
    {
        currentField = field;
        foreach(var effectiveField in FindObjectsOfType<GravityField>())
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
        if (other.gameObject.name.Equals("DestructionBounds"))
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
