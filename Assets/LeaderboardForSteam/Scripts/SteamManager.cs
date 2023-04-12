using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeastSquares;


public class SteamManager : MonoBehaviour
{
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX && !PLATFORM_STANDALONE_OSX
    [SerializeField] GameObject steamLeaderboard;
    [SerializeField] GameObject steamEngine;

    void Awake()
    {
        if (SteamLeaderboard.Instance != null)
            Destroy(gameObject);
        else {

            steamEngine.SetActive(true);
            steamLeaderboard.SetActive(true);
            
        }
    }
#endif
    
}
