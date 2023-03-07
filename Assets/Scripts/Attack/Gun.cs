using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform gunTip; 
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float bulletInterval = 0.2f;

    public void Shoot()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, gunTip.position, Quaternion.identity);
        Destroy(bullet, 5f);
        bullet.GetComponent<Bullet>().SetDirectionASpeed(transform.root.forward, speed);
    }

    private void Start()
    {
        InvokeRepeating("Shoot", 0f, bulletInterval);
    }

}
