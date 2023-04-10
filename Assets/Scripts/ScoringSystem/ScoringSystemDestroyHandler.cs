using System;
using UnityEngine;

public class ScoringSystemDestroyHandler : MonoBehaviour
{
    private void Awake()
    {
        GameObject go = GameObject.FindWithTag("ScoringSystem");
        if (go)
        {
            Destroy(go);
        }
    }
}