using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float speed =20;
    Vector3 direction;
    [SerializeField]
    int damage = 1; 
    
    private void Update()
    {
        Fly(direction, speed);
    }

    private void Fly(Vector3 direction, float speed)
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    public void SetDirectionAndSpeed(Vector3 direction, float speed) {
        this.speed = speed;
        this.direction = direction; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))// do damage to player/enemy
        {
            other.gameObject.Trigger<IHealthTriggers, int>(nameof(IHealthTriggers.TakeDamage), damage);
        }
        if (!other.gameObject.CompareTag("Bullet")) {
            Destroy(gameObject);
        }
    }
}
