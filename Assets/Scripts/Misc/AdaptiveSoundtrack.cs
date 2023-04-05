using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveSoundtrack : MonoBehaviour
{
    [SerializeField] float[] thresholds;
    [SerializeField] AudioSource[] layers;
    public Health pHealth;
    public ShotCounter sc;
    [SerializeField] AudioSource healthLayer;
    [SerializeField] float healthThreshold;
    [SerializeField] bool mute;
    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioSource a in layers) { a.volume = 0; }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!mute)
        {
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (layers[i].volume == 0 && sc.shots >= thresholds[i])
                {
                    EnableSnd(layers[i]);
                }
                else if (layers[i].volume == 1 && sc.shots < thresholds[i])
                {
                    DisableSnd(layers[i]);
                    break;
                }
            }
            if (pHealth.currentHealth <= healthThreshold && healthLayer.volume == 0)
            {
                EnableSnd(healthLayer);
            }
            else if (pHealth.currentHealth > healthThreshold && healthLayer.volume == 1)
            {
                DisableSnd(healthLayer);
            }
        }
    }
    void EnableSnd(AudioSource asource) {
        asource.volume = 1;
    }
    void DisableSnd(AudioSource asource){
        asource.volume = 0;
    }
}
