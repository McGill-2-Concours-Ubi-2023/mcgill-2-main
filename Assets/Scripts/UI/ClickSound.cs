using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSound : MonoBehaviour
{
    [SerializeField]AudioSource AS;
    [SerializeField]bool dontRandomize;
    [SerializeField]float soundGap = 1;
    float startPitch;

    float nextsoundtime = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(AS==null)
            AS = gameObject.GetComponent<AudioSource>();
        startPitch = AS.pitch;
    }

    public void Click()
    {
        if(!dontRandomize)
            AS.pitch = startPitch* Random.Range(0.95f, 1.05f);
        if(Time.time>nextsoundtime){
            AS.PlayOneShot(AS.clip);
            nextsoundtime = Time.time+soundGap;
        }
    }

    public void Click(AudioClip ac)
    {
        if (!dontRandomize)
            AS.pitch = startPitch* Random.Range(0.95f, 1.05f);
        if (Time.time > nextsoundtime)
        {
            AS.PlayOneShot(ac);
            nextsoundtime = Time.time + soundGap;
        }
    }
}
