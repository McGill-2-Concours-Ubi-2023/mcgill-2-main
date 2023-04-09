using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWaves : MonoBehaviour
{

    public float deformSpeed = .2f;
    float[] randomTimes;
    List<Vector3> newVertices;
    List<Vector3> startVertices;
    Mesh mesh;

    public bool deform = false;
    public bool basedOnXYpos = false;
    public Vector2 deformRange = new Vector2(-.5f, .5f);//low point and high point along vertical axis

    public bool distortUVs = true;
    public Vector2 uvDistancementAmount = new Vector2(1, 1);

    public float deformNoisinessIfXYpos = 10f;

    public bool scrollTexture = true;
    public float horizontalScrollSpeed = 0.25f;
    public float verticalScrollSpeed = 0.25f;

    private bool warmedUp = false;
    public IEnumerator Start()
    {
        if (deform || distortUVs)
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        //assigns each mesh vertex a time offset to animate along
        if (deform)
        {
            newVertices = new List<Vector3>();
            startVertices = new List<Vector3>();

            mesh.GetVertices(newVertices);
            mesh.GetVertices(startVertices);
            randomTimes = new float[newVertices.Count];
            for (int i = 0; i < newVertices.Count; i++)
            {
                
                if (basedOnXYpos)
                {
                    randomTimes[i] = deformNoisinessIfXYpos * ((Mathf.Sin(newVertices[i].x) + Mathf.Sin(newVertices[i].z) + Mathf.Sin(newVertices[i].y)));
                }
                else
                {
                    randomTimes[i] = Random.Range(-5f, 5f);
                }
                yield return null;
            }
        }

        //moves UV points in random directions. used to break up repetitive planar mapping
        if (distortUVs && uvDistancementAmount != Vector2.one)
        {
            Vector2[] uvs = mesh.uv;
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] += new Vector2(Random.value * uvDistancementAmount.x, Random.value * uvDistancementAmount.y);
            }
            mesh.uv = uvs;
        }

        materialTarget = this.GetComponent<Renderer>();
        warmedUp = true;
        //make sure reflection texture scales with tiling on albedo (for tiling glitch in render pipeline)
        //materialTarget.material.SetTextureScale("_ReflectionTexture", materialTarget.material.GetTextureScale("_MainTex"));
    }


    Renderer materialTarget;
    public void Update()
    {
        if (warmedUp)
        {
            if (deform)
            {
                for (int i = 0; i < newVertices.Count; i++)
                {
                    var vertex = newVertices[i];
                    vertex.y = startVertices[i].y + Mathf.Lerp(deformRange.x, deformRange.y,(Mathf.PingPong((Time.time + randomTimes[i]) * deformSpeed, 1f)));
                    newVertices[i] = vertex;
                }
                mesh.SetVertices(newVertices);
            }

            if (scrollTexture)
            {
                float verticalOffset = Time.time * verticalScrollSpeed;
                float horizontalOffset = Time.time * horizontalScrollSpeed;
                materialTarget.material.SetTextureOffset("_MainTex", new Vector2(horizontalOffset, verticalOffset));
                materialTarget.material.SetTextureOffset("_ReflectionTexture", new Vector2(horizontalOffset, verticalOffset));
            }
        }
    }

    public void ToggleScroll()
    {
        scrollTexture = !scrollTexture;
    }

}