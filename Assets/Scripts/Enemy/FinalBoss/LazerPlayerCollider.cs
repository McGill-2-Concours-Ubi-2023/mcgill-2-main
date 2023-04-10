using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerPlayerCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(1);
        }
    }
}
