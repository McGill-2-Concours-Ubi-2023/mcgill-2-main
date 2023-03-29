using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct ApplyConcentricGravityJob : IJob
{
    private Vector3 fieldPosition;
    private Vector3 upVector;
    private Quaternion rbRotation;
    private Vector3 rbPosition;
    private bool isLayerMask;
    private float deltaTime;
    private float orientationSpeed;
    private float mediumDensity;
    private float attractionForce;
    private float gravity;
    private float dampeningForce;
    private Vector3 rbVelocity;
    private int index;
    private NativeArray<Vector3> velocityResult;
    private NativeArray<Quaternion> rotationResult;

    public ApplyConcentricGravityJob(Vector3 _rbPosition,
        Quaternion _rbRotation,
        bool _isLayerMask,
        float _deltaTime,
        Vector3 _fieldPosition,
        Vector3 _upVector,
        float _orientationSpeed,
        float _mediumDensity,
        float _attractionForce,
        float _gravity,
        Vector3 _velocity,
        float _dampeningForce,
        NativeArray<Vector3> _velocityResult,
        NativeArray<Quaternion> _rotationResult,
        int _index
        )
    {
        rbPosition = _rbPosition;
        rbRotation = _rbRotation;
        isLayerMask = _isLayerMask;
        deltaTime = _deltaTime;
        fieldPosition = _fieldPosition;
        upVector = _upVector;
        orientationSpeed = _orientationSpeed;
        mediumDensity = _mediumDensity;
        attractionForce = _attractionForce;
        gravity = _gravity;
        rbVelocity = _velocity;
        dampeningForce = _dampeningForce;
        velocityResult = _velocityResult;
        rotationResult = _rotationResult;
        index = _index;
    }
    public void Execute()
    {
        var forceDirection = fieldPosition - rbPosition;
        Debug.DrawRay(rbPosition, forceDirection, Color.red);
        //First generate the quaternion (similar to a transformation matrix)
        Quaternion rotationQuaternion = Quaternion.FromToRotation(upVector, forceDirection);
        //Multiply the agent's rotation (also a quaternion) by the above quaternion
        Quaternion targetRotation = rotationQuaternion * rbRotation;
        //Finally, smoothly interpolate between the current rotation and the target rotation
        if (isLayerMask)
            rbRotation = Quaternion.Lerp(rbRotation, targetRotation, orientationSpeed * deltaTime);
        //When approaching Kernel, attraction force intensifies
        var FgLerp = Mathf.Lerp(mediumDensity + attractionForce, mediumDensity, forceDirection.magnitude);
        var velocityChange = forceDirection.normalized * gravity * FgLerp * deltaTime;
        rbVelocity += velocityChange;
        //Dampen over time to prevent sinusoidal behaviour
        var dampenLerp = Mathf.Lerp(0, dampeningForce, Mathf.Clamp01(forceDirection.magnitude));
        var velocityDampening = -dampenLerp * rbVelocity * deltaTime;
        rbVelocity += velocityDampening;
        velocityResult[index] = rbVelocity;
        rotationResult[index] = rbRotation;
    }
}

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
    private float radius;
    private JobHandle _jobHandle;
    private NativeArray<Vector3> velocityResults;
    private NativeArray<Quaternion> rotationResults;

    private void Awake()
    {
        velocityResults = new NativeArray<Vector3>(30, Allocator.Persistent);
        rotationResults = new NativeArray<Quaternion>(30, Allocator.Persistent);
        radius = GetComponent<SphereCollider>().radius;
    }

    public float Radius()
    {
        return radius;
    }

    protected override void ApplyGravity(Rigidbody rb)
    {
        var forceDirection = transform.position - rb.transform.position;
        Debug.DrawRay(rb.transform.position, forceDirection, Color.red);
        //First generate the quaternion (similar to a transformation matrix)
        Quaternion rotationQuaternion = Quaternion.FromToRotation(transform.up, forceDirection);
        //Multiply the agent's rotation (also a quaternion) by the above quaternion
        Quaternion targetRotation = rotationQuaternion * rb.transform.rotation;
        //Finally, smoothly interpolate between the current rotation and the target rotation
        if(rb.gameObject.layer != LayerMask.NameToLayer("Player") && !rb.CompareTag("Enemy"))
        rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, targetRotation, orientationSpeed * Time.deltaTime);
        //When approaching Kernel, attraction force intensifies
        var FgLerp = Mathf.Lerp(mediumDensity + attractionForce, mediumDensity, forceDirection.magnitude);
        var velocityChange = forceDirection.normalized * gravity * FgLerp * Time.deltaTime;
        rb.velocity += velocityChange;
        //Dampen over time to prevent sinusoidal behaviour
        var dampenLerp = Mathf.Lerp(0, dampeningForce, Mathf.Clamp01(forceDirection.magnitude));
        var velocityDampening = -dampenLerp * rb.velocity * Time.deltaTime;
        rb.velocity += velocityDampening;        
    }

    /*protected override void ApplyGravityJob()
    {
        JobHandle dependency = _jobHandle;
        int i;
        for(i = 0; i < cachedRigidbodies.Count; i++)
        {
            bool isLayerMask = cachedRigidbodies[i].gameObject.layer != LayerMask.NameToLayer("Player") 
                && !cachedRigidbodies[i].CompareTag("Enemy");
            ApplyConcentricGravityJob job = new ApplyConcentricGravityJob(cachedRigidbodies[i].position,
                cachedRigidbodies[i].rotation, isLayerMask, Time.deltaTime,
                transform.position, transform.up, orientationSpeed, mediumDensity, attractionForce, gravity,
                cachedRigidbodies[i].velocity, dampeningForce, velocityResults, rotationResults, i);
            dependency = job.Schedule(dependency);
        }
        _jobHandle = dependency;
        _jobHandle.Complete(); // Wait for the job to complete
        UpdateRigidbodies();
    }*/


    /*private void UpdateRigidbodies()
    {
        _jobHandle.Complete();
        int i;
        for(i = 0; i < cachedRigidbodies.Count; i++)
        {
            cachedRigidbodies[i].velocity = velocityResults[i];
            cachedRigidbodies[i].rotation = rotationResults[i];
        }
    }*/

    /*protected override void DetectCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            ProcessCollision(collider);
        }
    }*/

    private void OnDestroy()
    {
        velocityResults.Dispose();
        rotationResults.Dispose();
    }
}

