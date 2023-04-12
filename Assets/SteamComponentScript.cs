using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamComponentScript : MonoBehaviour
{

    public static SteamComponentScript Instance;


    private void Awake()
    {
        Debug.Log("Awakening");
            //DontDestroyOnLoad(this);

            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

}
