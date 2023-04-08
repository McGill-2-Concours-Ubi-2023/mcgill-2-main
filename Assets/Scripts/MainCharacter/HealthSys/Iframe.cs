using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iframe : MonoBehaviour
{
    [SerializeField]
    private float duration;
    [SerializeField]
    private int numOfFlashes;
    [SerializeField]
    private SkinnedMeshRenderer characterBodySR;
    [SerializeField]
    private SkinnedMeshRenderer characterHairSR;
    [SerializeField]
    private Health playerHealth;
    [SerializeField]
    private Material flashMaterial;

    private Material originalBodyMaterial;
    private Material originalHairMaterial;
   /* private Color originalBodyColor;
    private Color originalHairColor;*/

    // Update is called once per frame
    void Update()
    {
        //test code TODO: DELETE THIS
       /* if (Input.GetKeyDown(KeyCode.P))
        {
            playerHealth.TakeDamage(1);
        }*/
    }

    private void Start()
    {
        playerHealth.OnHealthChange += flash;
        originalBodyMaterial = characterBodySR.material;
        originalHairMaterial = characterHairSR.material;
    }

    public void flash(float change, float currentHealth)
    {
        if (change >= 0) return;
        StartCoroutine(flash());
    }

    private IEnumerator flash()
    {

        int i = 0;
        float timer = 0f;
        using HLockGuard healthLock = playerHealth.Lock();
        while (i < numOfFlashes)
        {
            if (timer < duration)
            {
                characterBodySR.material = flashMaterial;
                characterHairSR.material = flashMaterial;
            }
            else if (timer < duration * 2)
            {
                characterBodySR.material = originalBodyMaterial;
                characterHairSR.material = originalHairMaterial;
            }
            else
            {
                timer = 0f;
                i++;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
