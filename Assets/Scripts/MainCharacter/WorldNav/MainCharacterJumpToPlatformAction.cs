using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/**
 * PLACEHOLDER
 */
public struct MainCharacterJumpToPlatformAction : INavAction
{
    public float MaxDistance;
    public RaycastHit Hit;
    
    public bool ShouldTransition(GameObject gameObject)
    {
        float3 pos = gameObject.transform.position;
        pos.y += 1.5f;
        float3 dir = gameObject.transform.forward;
        
        // find platform ahead of player
        // generate a series of rays
        // if any ray hits a platform, jump to it
        const int numRays = 30;
        List<RaycastHit> hits = new List<RaycastHit>(numRays);
        for (int i = 0; i < numRays; i++)
        {
            float3 rayDir = dir;
            rayDir.y += (i - numRays * 0.7f) * (1.0f / numRays);
            rayDir = math.normalize(rayDir) * MaxDistance;
            //Debug.DrawRay(pos, rayDir, Color.green);
            if (Physics.Raycast(pos, rayDir, out RaycastHit hit, MaxDistance))
            {
                hits.Add(hit);
            }
        }

        int lowBound = 0;
        int highBound = hits.Count - 1;
        if (lowBound == highBound)
        {
            return false;
        }
        RaycastHit? targetHit = null;
        while (lowBound < highBound)
        {
            RaycastHit lowHit = hits[lowBound];
            RaycastHit highHit = hits[highBound];
            float3 lowNormal = lowHit.normal;
            float3 highNormal = highHit.normal;
            float lowAngle = Vector3.Angle(Vector3.up, lowNormal);
            float highAngle = Vector3.Angle(Vector3.up, highNormal);
            if (lowAngle > 60 && highAngle < 30)
            {
                targetHit = hits[(lowBound + highBound) / 2];
                if (Vector3.Angle(Vector3.up, hits[lowBound + 1].normal) < 30)
                {
                    targetHit = hits[lowBound + 1];
                    break;
                }
                if (Vector3.Angle(Vector3.up, hits[highBound - 1].normal) > 60)
                {
                    targetHit = hits[highBound];
                    break;
                }
                
            }
            
            lowBound++;
            highBound--;
        }

        if (targetHit.HasValue)
        {
            Hit = targetHit.Value;
            Debug.DrawLine(gameObject.transform.position, Hit.point, Color.red, 1.0f);
            return true;
        }

        return false;
    }
}

