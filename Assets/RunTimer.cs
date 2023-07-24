using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("runstart", Time.time);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
