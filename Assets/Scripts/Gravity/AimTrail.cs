using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTrail : MonoBehaviour
{
    private const float g = 9.81f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Theta = angle between xz plane and y component of velocity
    //Phi = angle between xy plane and z component of velocity
    public void ComputeTrajectory(float v0, float theta, float phi, float g)
    {
        var x = v0 * Time.time * Mathf.Cos(theta) * Mathf.Cos(phi);
        var y = v0 * Time.time * Mathf.Cos(theta) * Mathf.Sin(phi) - (1 / 2) * g * Mathf.Pow(Time.time, 2);
        var z = v0 * Time.time * Mathf.Sin(theta);
    }
}
