using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLight : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Flicker()
    {
        StartCoroutine(TriggerFlickerEvent());
    }

    private IEnumerator TriggerFlickerEvent()
    {
        float randomWait = UnityEngine.Random.Range(8, 16);
        yield return new WaitForSeconds(randomWait);
        int randomIndex = UnityEngine.Random.Range(1, 4);
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetTrigger(randomIndex.ToString());
    }
    
    public void TurnRed()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetTrigger("TurnRed");
    }

    public void ResetColor()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetTrigger("ResetColor");
    }
}
