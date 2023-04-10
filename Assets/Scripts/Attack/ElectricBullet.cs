using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public VisualEffect electricEffect;
    private void OnDestroy()
    {
        var effect = Instantiate(electricEffect);
        effect.transform.position = transform.position;
    }
}
