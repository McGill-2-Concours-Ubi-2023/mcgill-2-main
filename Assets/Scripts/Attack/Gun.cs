using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;

public interface IGunTriggers : ITrigger
{
    void OnShootStartIntention();
    void OnShootStopIntention();
    void IncreaseFireRate(float amount);
    void ChangeBullet();
}

public class Gun : MonoBehaviour, IGunTriggers
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform gunTip; 
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float bulletInterval = 0.2f;

    ClickSound cs;
    ShotCounter sc;
    
    private Coroutine m_ShootCoroutine;
    private bool m_ShootCoroutinePaused = true;
    [SerializeField] bool overrideVel = true;
    [SerializeField] bool playerGun;
    [SerializeField]
    private GameObject altBullet; 
    private MainCharacterController playerController;
    private Vibration vibration;


    private void Start()
    {
        if (playerGun)
            playerController = GetComponentInParent<MainCharacterController>();
        cs = GetComponent<ClickSound>();
        sc= GetComponent<ShotCounter>();
        vibration = GameObject.Find("GamepadVib").GetComponent<Vibration>();
    }

    private void Shoot()
    {
        if (playerGun)
        {
            //GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position + transform.root.forward * 0.2f, Quaternion.identity);
            // ray cast forward with an angle of 15 degrees to find enemies
            // if enemy is found, shoot at enemy
            
            // find all enemies with cylinder cast
            HashSet<GameObject> enemies = new HashSet<GameObject>();
            float3 forward = transform.root.forward;
            // sweep left and right 15 degrees
            RaycastHit[] hits = new RaycastHit[5];
            for (float angle = -15; angle <= 15; angle += 3)
            {
                // rotate forward left by angle
                float3 dir = mul(Quaternion.AxisAngle(up(), radians(angle)), forward);
                // cast ray
                int numHits = Physics.SphereCastNonAlloc(gunTip.position, 0.1f, dir, hits, 15f);
                Debug.DrawRay(gunTip.position, dir * 15f, Color.magenta);
                for (int i = 0; i < numHits; i++)
                {
                    GameObject enemy = hits[i].collider.gameObject;
                    if (enemy.CompareTag("Enemy"))
                    {
                        enemies.Add(enemy);
                    }
                }
            }
            
            // find closest enemy
            GameObject closestEnemy = null;
            float minDist = float.MaxValue;
            foreach (GameObject enemy in enemies)
            {
                float dist = length(enemy.transform.position - gunTip.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemy = enemy.transform.root.gameObject;
                }
            }
            
            Debug.Log($"Closest enemy count: {enemies.Count}");
            
            // if enemy is found, shoot at enemy
            if (closestEnemy)
            {
                Debug.Log($"Closest enemy: {closestEnemy.name} at {closestEnemy.transform.position}");
                Debug.DrawLine(gunTip.position, closestEnemy.transform.position, Color.magenta, 2f);
            }
    
            GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
            float3 vel;
            if (closestEnemy)
            {
                vel = normalize(closestEnemy.transform.position - gunTip.position) * speed;
            }
            else
            {
                vel = transform.root.forward * speed;
            }
            if (bulletPrefab != altBullet)
            {
                bullet.GetComponent<Bullet>().SetDirectionAndSpeed(normalize(vel), length(vel));
            }
            else
            {
                Debug.Log("Alt bullet");
            }
            playerController.GetComponent<Animator>().SetTrigger("Shoot");
        }
        else
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
            float3 vel = transform.root.GetComponent<Rigidbody>().velocity;
            vel += (float3)(transform.root.forward * speed);
            if (overrideVel)
                bullet.GetComponent<Bullet>().SetDirectionAndSpeed(normalize(vel), length(vel));
        }
        
        if (cs != null)
            cs.Click();
        if (sc != null)
            sc.shotsFired();
        vibration.SoftVibration();
    }
    
    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            Ref<bool> refPlayerIsDashing = false;
            gameObject.TriggerUp<IMainCharacterTriggers, Ref<bool>>(nameof(IMainCharacterTriggers.IsDashing), refPlayerIsDashing);
            if (!refPlayerIsDashing)
            {
                Shoot();
            }
            yield return new WaitForSeconds(bulletInterval);
            while (m_ShootCoroutinePaused)
            {
                yield return null;
            }
        }
    }

    public void OnShootStartIntention()
    {
        m_ShootCoroutinePaused = false;
        m_ShootCoroutine ??= StartCoroutine(ShootCoroutine());
    }
    
    public void OnShootStopIntention()
    {
        Assert.IsNotNull(m_ShootCoroutine);
        m_ShootCoroutinePaused = true;
    }

    public void IncreaseFireRate(float amount) {
        this.bulletInterval = this.bulletInterval / amount;
    }

    public void ChangeBullet() {
        this.bulletPrefab = altBullet;
    }



}
