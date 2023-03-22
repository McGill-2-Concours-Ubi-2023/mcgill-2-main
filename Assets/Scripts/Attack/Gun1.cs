using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;


public class Gun1 : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform gunTip; 
    [SerializeField]
    private float bulletInterval = 0.2f;
    private float nextshot=0;


    private void Start()
    {
    }

    public void Shoot()
    {
        if (Time.time >= nextshot)
        {
            GameObject.Instantiate(bulletPrefab, gunTip.position, gunTip.rotation);
            nextshot = Time.time + bulletInterval;
        }
    }
}
