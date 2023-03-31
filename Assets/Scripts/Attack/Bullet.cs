using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    Vector3 direction;
    [SerializeField]
    int damage = 1;
    int gravityLayer;

    private void Update()
    {
        Fly(direction, speed);
    }

    private void OnEnable()
    {
        gravityLayer = LayerMask.NameToLayer("GravityField");
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
        if(gameObject.CompareTag("EnemyBullet"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.Trigger<IHealthTriggers, int>(nameof(IHealthTriggers.TakeDamage), damage);
            }
        }
        else if (gameObject.CompareTag("PlayerBullet"))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log(other.name);
                other.gameObject.Trigger<IHealthTriggers, int>(nameof(IHealthTriggers.TakeDamage), damage);
            }
        }
        
        if (!(other.gameObject.CompareTag("EnemyBullet") 
            && other.gameObject.CompareTag("PlayerBullet"))
            && other.gameObject.layer != gravityLayer) {
            Destroy(gameObject);
        }
    }
}
