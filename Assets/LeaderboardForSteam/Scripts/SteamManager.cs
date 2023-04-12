using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeastSquares;


public class SteamManager : MonoBehaviour
{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
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
