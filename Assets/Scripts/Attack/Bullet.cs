using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    Vector3 direction;
    [SerializeField]
    int damage = 1;
    int gravityLayer;
    [SerializeField]
    public VisualEffect shieldImpactVFX;


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
        transform.forward = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.CompareTag("EnemyBullet"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.Trigger<IHealthTriggers, float>(nameof(IHealthTriggers.TakeDamage), damage);
                other.gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnDamageCameraShake));
            }
        }
        else if (gameObject.CompareTag("PlayerBullet"))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.Trigger<IHealthTriggers, float>(nameof(IHealthTriggers.TakeDamage), damage);
            }
            else if (other.gameObject.transform.root.gameObject.CompareTag("FinalBoss"))
            {
                other.gameObject.TriggerUp<IHealthTriggers, float>(nameof(IHealthTriggers.TakeDamage), damage);
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("BossShield"))
        {
            Debug.Log("HIT");
            VisualEffect shieldVFX = Instantiate(shieldImpactVFX);
            shieldVFX.transform.position = transform.position;
            StartCoroutine(OnShieldImpact(shieldVFX));
        }

        if (!(other.gameObject.CompareTag("EnemyBullet") 
            && other.gameObject.CompareTag("PlayerBullet"))
            && other.gameObject.layer != gravityLayer) {
            Destroy(gameObject);
        }       
    }

    IEnumerator OnShieldImpact(VisualEffect vfx)
    {
        vfx.SendEvent("OnShieldImpact");
        yield return new WaitForSeconds(2.0f);
        Destroy(vfx.gameObject);
    }
}
