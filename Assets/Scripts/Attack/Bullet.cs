using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed;
    Vector3 direction;
    [SerializeField]
    int damage = 1; 

    // Update is called once per frame
    void Update()
    {
        fly(direction, speed);
    }

    public void fly(Vector3 direction, float speed)
    {
        this.GetComponent<Rigidbody>().velocity = direction * speed;
    }

    public void SetDirectionASpeed(Vector3 direction, float speed) {
        this.speed = speed;
        this.direction = direction; 
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collide"+ other.name);
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))// do damage to player/enemy
        {
            other.gameObject.GetComponent<Health>().TakeDamage(damage); 
        }
        if (!other.CompareTag("Bullet")) {
            GameObject.Destroy(this.gameObject);
        }
        
    }
}
