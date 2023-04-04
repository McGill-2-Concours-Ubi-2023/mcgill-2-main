using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotCounter : MonoBehaviour
{
    public float shots = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shots = Mathf.Clamp(shots - Time.deltaTime*2, 0, shots);
    }
    public void shotsFired() {
        shots = Mathf.Clamp(shots +1, 0, 20);
    }
}
