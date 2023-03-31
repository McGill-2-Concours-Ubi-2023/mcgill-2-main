using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public GameObject target;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target) //if arm hits the player
        {
            //Unlucky, get better and dodge, stop blaming the developers for your lack of skills
            target.Trigger<IHealthTriggers, int>(nameof(IHealthTriggers.TakeDamage), 1);
        }
    }
}
