using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAgent : MonoBehaviour, IGravityTriggers
{

    private bool hasEscaped;
    private float massCompression;

    public void SetMassCompression(float massCompression)
    {
        Debug.Log("APPLYING COMPRESSION!!");
        this.massCompression = massCompression;
    }

    public IEnumerator OnWaitDestroy(float timer, GameObject trigger)
    {
        yield return new WaitForSeconds(timer);
        if (!hasEscaped)
        {
            StartCoroutine(Disappear(trigger));
        }
    }

    private IEnumerator Disappear(GameObject trigger)
    {
        float timer = 0; 
        while(transform.localScale.magnitude > 0.25f) 
        {
            //Exponentially decrease the scale
            var decreaseFactor = Mathf.Exp(timer * massCompression) * Time.deltaTime;
            //Update the scale
            transform.localScale -= transform.localScale * decreaseFactor;
            timer += Time.deltaTime;
            yield return null;
        }
        trigger.GetComponentInParent<GravityField>().ReleaseAgent(this.gameObject);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("DestructionBounds"))
        {
            hasEscaped = false;
            StartCoroutine(OnWaitDestroy(other.gameObject.GetComponentInParent<GravitationalGrenade>()
                .GetDestructionTimer(), other.gameObject));         
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("DestructionBounds"))
        {
            hasEscaped = true;
            StopAllCoroutines();
        }
    }
}
