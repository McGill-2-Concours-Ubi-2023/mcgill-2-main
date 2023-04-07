using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingText : MonoBehaviour
{
    [SerializeField] float flashSpeedOn;
    [SerializeField] float flashSpeedOff;

    void Start()
    {
        StartCoroutine("TextFlashing");
    }
 
    public IEnumerator TextFlashing()
    {
        GameObject text = transform.GetChild(0).gameObject;

        while (true)
        {
            text.SetActive(false);
            yield return new WaitForSeconds(flashSpeedOff);
            text.SetActive(true);
            yield return new WaitForSeconds(flashSpeedOn);
        }
    }
}
