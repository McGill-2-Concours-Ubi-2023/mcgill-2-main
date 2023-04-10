using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxPosition : MonoBehaviour
{
    public Vector2 position;

    [SerializeField] float max;
    [SerializeField] float min;
    [SerializeField] float time;

    
    void Start()
    {
        position = new Vector2(0,0);
        StartCoroutine("RandomBgMovement");
    }
 
    public IEnumerator RandomBgMovement()
    {

        System.Random random = new System.Random();

        while (true)
        {
            position = new Vector2((float) random.NextDouble() * (max - min) + min, (float) random.NextDouble() * (max - min) + min);
            yield return new WaitForSeconds(time);
        }
    }

}
