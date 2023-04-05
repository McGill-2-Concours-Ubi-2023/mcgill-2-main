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
    [SerializeField] bool overrideVel;
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
        //GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position + transform.root.forward * 0.2f, Quaternion.identity);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
        float3 vel = transform.root.GetComponent<Rigidbody>().velocity;// player velocity
        vel += (float3)(transform.root.forward * speed);
        if(overrideVel)
            bullet.GetComponent<Bullet>().SetDirectionAndSpeed(normalize(vel), length(vel));
        if(playerGun)
            playerController.GetComponent<Animator>().SetTrigger("Shoot");
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
