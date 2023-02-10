using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcentricGravityField : GravityField
{
    [Range(1, 10)]
    public float orientationSpeed;
    [Range(0.001f, 1f)]
    public float dampeningForce = 0.2f;
    [Range(1f, 20f)]
    public float attractionForce;
    [Range(0.1f, 5.0f)]
    public float mediumDensity;

    protected override void ApplyGravity(Rigidbody rb)
    {
        var forceDirection = transform.position - rb.transform.position;
        Debug.DrawRay(rb.transform.position, forceDirection, Color.red);
        //First generate the quaternion (similar to a transformation matrix)
        Quaternion rotationQuaternion = Quaternion.FromToRotation(-rb.transform.up, forceDirection);
        //Multiply the agent's rotation (also a quaternion) by the above quaternion
        Quaternion targetRotation = rotationQuaternion * rb.transform.rotation;
        //Finally, smoothly interpolate between the current rotation and the target rotation
        rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, targetRotation, orientationSpeed * Time.deltaTime);

        //When approaching Kernel, attraction force intensifies
        var FgLerp = Mathf.Lerp(mediumDensity + attractionForce, mediumDensity, forceDirection.magnitude);
        var velocityChange = forceDirection.normalized * gravity * rb.mass * FgLerp * Time.deltaTime;
        rb.velocity += velocityChange;
        //Dampen over time to prevent sinusoidal behaviour
        var dampenLerp = Mathf.Lerp(0, dampeningForce, Mathf.Clamp01(forceDirection.magnitude));
        var velocityDampening = -dampenLerp * rb.velocity * Time.deltaTime;
        rb.velocity += velocityDampening;
    }
}

