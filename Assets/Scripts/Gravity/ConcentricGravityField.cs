using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ApplyBulletGravityJob :IJobParallelFor
{
    private float3 fieldPosition;
    private float deltaTime;
    private float orientationSpeed;
    private float mediumDensity;
    private float attractionForce;
    private float gravity;
    private float dampeningForce;
    private NativeArray<float3> velocityArray;
    private NativeArray<Quaternion> rotationArray;
    private NativeArray<float3> positionArray;
    private NativeArray<float3> rotationAxisArray;
    private float indirectForce;
    private bool adjustRotation;

    public ApplyBulletGravityJob(
        float _deltaTime,
        float3 _fieldPosition,
        float _orientationSpeed,
        float _mediumDensity,
        float _attractionForce,
        float _gravity,
        float _dampeningForce,
        NativeArray<float3> _velocityArray,
        NativeArray<Quaternion> _rotationArray,
        NativeArray<float3> _positionArray,
        NativeArray<float3> _forwardVectorArray,
        float _indirectForce,
        bool _adjustRotation
        )
    {
        deltaTime = _deltaTime;
        fieldPosition = _fieldPosition;
        orientationSpeed = _orientationSpeed;
        mediumDensity = _mediumDensity;
        attractionForce = _attractionForce;
        gravity = _gravity;
        dampeningForce = _dampeningForce;
        velocityArray = _velocityArray;
        rotationArray = _rotationArray;
        positionArray = _positionArray;
        rotationAxisArray = _forwardVectorArray;
        indirectForce = _indirectForce;
        adjustRotation = _adjustRotation;

    }
    public void Execute(int index)
    {
        var forceDirection = fieldPosition - positionArray[index];
        Debug.DrawRay(positionArray[index], forceDirection, Color.red);

        //Generate Quaternion transform
        Quaternion rotationQuaternion;
        //float indirectForce = BulletAgent.bulletBendingForce;
        rotationQuaternion = Quaternion.FromToRotation(rotationAxisArray[index], forceDirection);
        //Multiply the agent's rotation by quaternion transform
        Quaternion targetRotation = rotationQuaternion * rotationArray[index];
        bool nullQuaternion = rotationArray[index].x == 0 &&  rotationArray[index].y == 0
            && rotationArray[index].z == 0 && rotationArray[index].w == 0;

        if (nullQuaternion) rotationArray[index] = Quaternion.identity;
        if(adjustRotation)
        rotationArray[index] = Quaternion.Lerp(rotationArray[index], targetRotation, orientationSpeed * deltaTime);

        //When approaching Kernel, attraction force intensifies
        float forceMag = ((Vector3)forceDirection).magnitude;
        var FgLerp = Mathf.Lerp(mediumDensity + attractionForce, mediumDensity, forceMag);
        var velocityChange = forceDirection * gravity * FgLerp * deltaTime * indirectForce;
        velocityArray[index] += velocityChange;

        //Dampen over time to prevent sinusoidal behaviour
        var dampenLerp = Mathf.Lerp(0, dampeningForce, Mathf.Clamp01(forceMag));
        var velocityDampening = -dampenLerp * velocityArray[index] * deltaTime;
        velocityArray[index] += velocityDampening;
    }
}

public class ConcentricGravityField : GravityField
{
    [Range(1, 10)]
    public float orientationSpeed;
    [Range(0.001f, 1f)]
    public float dampeningForce = 0.2f;
    [Range(0.1f, 5f)]
    public float attractionForce;
    [Range(0.1f, 5.0f)]
    public float mediumDensity;
    private float radius;

    private void Awake()
    {
        radius = GetComponent<SphereCollider>().radius;
    }

    public float Radius()
    {
        return radius;
    }

    protected override void ApplyGravity(Rigidbody rb)
    {
        float indirectForce = 1.0f; //None by default
        var forceDirection = transform.position - rb.transform.position;
        Debug.DrawRay(rb.transform.position, forceDirection, Color.red);

        //Generate Quaternion transform
        Quaternion rotationQuaternion = Quaternion.FromToRotation(transform.up, forceDirection);
        if (rb.gameObject.CompareTag("EnemyBullet") || rb.gameObject.CompareTag("PlayerBullet"))
        {
            indirectForce = BulletAgent.bulletBendingForce;
            rotationQuaternion = Quaternion.FromToRotation(transform.forward, forceDirection);
        }
        //Multiply the agent's rotation by quaternion transform
        Quaternion targetRotation = rotationQuaternion * rb.transform.rotation;
 
        //Rotate body accordingly                
        if (rb.gameObject.layer != fieldMask && !rb.CompareTag("Enemy"))
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, targetRotation, orientationSpeed * Time.deltaTime);

        //When approaching Kernel, attraction force intensifies
        var FgLerp = Mathf.Lerp(mediumDensity + attractionForce, mediumDensity, forceDirection.magnitude);
        var velocityChange = forceDirection.normalized * gravity * FgLerp * Time.deltaTime * indirectForce;
        rb.velocity += velocityChange;

        //Dampen over time to prevent sinusoidal behaviour
        var dampenLerp = Mathf.Lerp(0, dampeningForce, Mathf.Clamp01(forceDirection.magnitude));
        var velocityDampening = -dampenLerp * rb.velocity * Time.deltaTime;
        rb.velocity += velocityDampening;
    }
}

