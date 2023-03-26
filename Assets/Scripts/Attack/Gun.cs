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
    void ChangeBullet(GameObject bulletPrefab);
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
    
    private Coroutine m_ShootCoroutine;
    private bool m_ShootCoroutinePaused = true;
    [SerializeField] bool overrideVel;
    [SerializeField] bool playerGun;

    private MainCharacterController playerController;


    private void Start()
    {
        if (playerGun)
            playerController = GetComponentInParent<MainCharacterController>();
    }

    private void Shoot()
    {
        //GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position + transform.root.forward * 0.2f, Quaternion.identity);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
        float3 vel = transform.root.GetComponent<Rigidbody>().velocity;// player velocity
        vel += (float3)(transform.root.forward * speed);
        if(overrideVel)
            bullet.GetComponent<Bullet>().SetDirectionAndSpeed(normalize(vel), length(vel));
        if(playerGun)
            playerController.GetComponent<Animator>().SetTrigger("Shoot");
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
        if (m_ShootCoroutine == null)
        {
            m_ShootCoroutine = StartCoroutine(ShootCoroutine());
        }
        else
        {
            m_ShootCoroutinePaused = false;
        }
    }
    
    public void OnShootStopIntention()
    {
        Assert.IsNotNull(m_ShootCoroutine);
        m_ShootCoroutinePaused = true;
    }

    public void IncreaseFireRate(float amount) {
        this.bulletInterval = this.bulletInterval / amount;
    }

    public void ChangeBullet(GameObject bulletPrefab) {
        this.bulletPrefab = bulletPrefab;
    }



}
