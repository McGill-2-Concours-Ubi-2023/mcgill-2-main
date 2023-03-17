using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iframes : MonoBehaviour
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
    private Color flashColor;

    private Color originalBodyColor;
    private Color originalHairColor;

    // Update is called once per frame
    void Update()
    {
        //test code TODO: DELETE THIS
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerHealth.TakeDamage(1);
        }
    }

    private void Start()
    {
        playerHealth.OnHealthChange += flash;
        originalBodyColor = characterBodySR.material.color;
        originalHairColor = characterHairSR.material.color;
    }

    public void flash(int change, int currentHealth)
    {
        if (change > 0) return;
        StartCoroutine(flash());
    }

    private IEnumerator flash()
    {

        int i = 0;
        float timer = 0f;
        playerHealth.invulnerable = true;
        while (i < numOfFlashes)
        {
            if (timer < duration)
            {
                characterBodySR.material.color = flashColor;
                characterHairSR.material.color = flashColor;
            }
            else if (timer < duration * 2)
            {
                characterBodySR.material.color = originalBodyColor;
                characterHairSR.material.color = originalHairColor;
            }
            else
            {
                timer = 0f;
                i++;
            }
            timer += Time.deltaTime;
            yield return null;

        }
        playerHealth.invulnerable = false;


    }
}