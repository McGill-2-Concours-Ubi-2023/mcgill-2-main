using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct Sample
{ 
    public NativeArray<float3> _velocityArray;
    public NativeArray<Quaternion> _rotationArray;
    public NativeArray<float3> _positionArray;
    public NativeArray<float3> _forwardVectorArray;
}

public enum RotationAxis
{
    Forward, Up
}

public class GravityAgentSample
{
    private Sample sample;
    public GravityAgentSample(int maxSampleSize, Allocator allocator)
    {
        sample = new Sample()
        {
            _velocityArray = new NativeArray<float3>(maxSampleSize, allocator),
            _rotationArray = new NativeArray<Quaternion>(maxSampleSize, allocator),
            _positionArray = new NativeArray<float3>(maxSampleSize, allocator),
            _forwardVectorArray = new NativeArray<float3>(maxSampleSize, allocator)
        };
    }

    public Sample Samples()
    {
        return sample;
    }

    public void UpdateSample(List<Rigidbody> rbList, RotationAxis axis) 
    {
        int i;
        for(i = 0; i < rbList.Count; i++)
        {
            if(rbList[i] != null)
            {
                sample._velocityArray[i] = rbList[i].velocity;
                sample._rotationArray[i] = rbList[i].rotation;
                sample._positionArray[i] = rbList[i].position;
                if(axis == RotationAxis.Forward)
                {
                    sample._forwardVectorArray[i] = rbList[i].transform.forward;
                }
                else if(axis == RotationAxis.Up)
                {
                    sample._forwardVectorArray[i] = rbList[i].transform.up;
                }
            }         
        }
    }

    public JobHandle InitializeGravityJob(float deltaTime, int arrayLength, int batchCount, float3 fieldPosition,
        float orientationSpeed, float mediumDensity, float attractionForce, float gravity,
        float dampeningForce, float indirectForce, bool adjustRotation)
    {
        ApplyBulletGravityJob gravityJob = new ApplyBulletGravityJob(deltaTime, fieldPosition,
           orientationSpeed, mediumDensity, attractionForce, gravity, dampeningForce,
           sample._velocityArray, sample._rotationArray, sample._positionArray, sample._forwardVectorArray, indirectForce, adjustRotation);
        return gravityJob.Schedule(arrayLength, batchCount);
    }

    public void RetrieveData(List<Rigidbody> rbList)
    {
        int i;
        for (i = 0; i < rbList.Count; i++)
        {
            if (rbList[i] != null)
            {
                rbList[i].velocity = sample._velocityArray[i];
                rbList[i].rotation = sample._rotationArray[i];
                rbList[i].position = sample._positionArray[i];
            }
        }
    }

    public void Dispose()
    {
        sample._positionArray.Dispose();
        sample._rotationArray.Dispose();
        sample._velocityArray.Dispose();
        sample._forwardVectorArray.Dispose();
    }
}
