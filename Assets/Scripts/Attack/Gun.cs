using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;

public interface IGunTriggers : ITrigger
{
    void OnShootStartIntention();
    void OnShootStopIntention();
    void IncreaseFireRate(float amount);
    void ChangeBulletShotGun();
    void ChangeBulletBig();
}

public struct GunState
{
    public GameObject bulletPrefab;
    public float speed;
    public float bulletInterval;
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
    private GameObject shotGunBullet, bigBullet; 
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
    
    [CanBeNull]
    public GameObject FindClosestEnemy()
    {
        GameAssistLevel assistLevel = GameManager.Instance.assistLevel;
        if (assistLevel == GameAssistLevel.Deactivated)
        {
            return null;
        }

        float maxAngle = assistLevel switch
        {
            GameAssistLevel.Default => 15.0f,
            GameAssistLevel.Enhanced => 50.0f,
            GameAssistLevel.Full => 180.0f,
            _ => 0.0f
        };

        //GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position + transform.root.forward * 0.2f, Quaternion.identity);
        // ray cast forward with an angle of 15 degrees to find enemies
        // if enemy is found, shoot at enemy
        
        GameObject player = GameObject.FindWithTag("Player");
            
        // find all enemies with cylinder cast
        HashSet<GameObject> enemies = new HashSet<GameObject>();
        float3 forward = transform.root.forward;
        // sweep left and right x degrees
        RaycastHit[] hits = new RaycastHit[5];
        for (float angle = -maxAngle; angle <= maxAngle; angle += 1)
        {
            // rotate forward left by angle
            float3 dir = mul(Quaternion.AxisAngle(up(), radians(angle)), forward);
            // cast ray
            float3 rayStart = (float3)player.transform.position + dir * 0.2f;
            int numHits = Physics.RaycastNonAlloc(rayStart, dir, hits, 15f);
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
                closestEnemy = enemy;
            }
        }
            
        Debug.Log($"Closest enemy count: {enemies.Count}");

        return closestEnemy;
    }

    private void Shoot()
    {
        if (playerGun)
        {
            GameObject closestEnemy = FindClosestEnemy();

            if (GameManager.Instance.assistLevel == GameAssistLevel.Full)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (closestEnemy)
                {
                    // turn to face enemy
                    float3 dir = normalizesafe(closestEnemy.transform.position - player.transform.position);
                    player.transform.forward = dir;
                    player.Trigger<IMainCharacterTriggers, float3>(nameof(IMainCharacterTriggers.OnAutoFaceIntention), dir);
                }
                else
                {
                    player.Trigger<IMainCharacterTriggers, float3>(nameof(IMainCharacterTriggers.OnAutoFaceIntention), float3(0.0f));
                }
            }

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
            if (bulletPrefab != shotGunBullet)
            {
                bullet.GetComponent<Bullet>().SetDirectionAndSpeed(normalize(vel), length(vel));
            }
            else
            {
                Debug.Log("shotgun bullet");
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
        m_ShootCoroutinePaused = true;
    }

    public void IncreaseFireRate(float amount) {
        this.bulletInterval = this.bulletInterval / amount;
    }

    public void ChangeBulletShotGun() {
        this.bulletPrefab = shotGunBullet;
    }

    public void ChangeBulletBig() {
        this.bulletPrefab = bigBullet;
    }

    public GunState ToSerializable()
        => new GunState
        {
            bulletPrefab = bulletPrefab,
            speed = speed,
            bulletInterval = bulletInterval
        };

    public void FromSerializable(GunState state)
    {
        bulletPrefab = state.bulletPrefab;
        speed = state.speed;
        bulletInterval = state.bulletInterval;
    }
}
