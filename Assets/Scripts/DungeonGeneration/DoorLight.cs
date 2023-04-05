using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLight : DungeonLight
{
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    public override void Flicker()
    {
        StartCoroutine(TriggerFlickerEvent());
    }

    private IEnumerator TriggerFlickerEvent()
    {
        float randomWait = UnityEngine.Random.Range(10, 20);
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
