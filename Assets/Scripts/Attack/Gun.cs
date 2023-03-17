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

    private void Start()
    {
        
    }

    private void Shoot()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position + transform.root.forward * 0.2f, Quaternion.identity);
        float3 vel = transform.root.GetComponent<Rigidbody>().velocity;
        vel += (float3)(transform.root.forward * speed);
        bullet.GetComponent<Bullet>().SetDirectionAndSpeed(normalize(vel), length(vel));
    }
    
    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            Ref<bool> refPlayerIsDashing = false;
            transform.root.gameObject.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.IsDashing), refPlayerIsDashing);
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
}
