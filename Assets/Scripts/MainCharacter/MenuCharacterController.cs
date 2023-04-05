using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharacterController : MonoBehaviour
{
    private Coroutine danceCoroutine;
    private Animator animator;
    private void Start()
    {
        StartCoroutine(RandomDance());
        animator = GetComponent<Animator>();

    }

    IEnumerator RandomDance()
    {
        while (true)
        {
            int randomEventTrigger = UnityEngine.Random.Range(6, 10);
            yield return new WaitForSeconds(randomEventTrigger);
            if (danceCoroutine != null) StopCoroutine(danceCoroutine);
            float onIdleWait = UnityEngine.Random.Range(6, 10);
            yield return new WaitForSeconds(onIdleWait);
            danceCoroutine = StartCoroutine(OnInputWait());
        }
    }

    IEnumerator OnInputWait()
    {
        float randomWait = UnityEngine.Random.Range(2, 4);
        while (randomWait > 0)
        {
            randomWait -= Time.deltaTime;
            yield return new WaitForEndOfFrame();            
        }
        int randomNumber = UnityEngine.Random.Range(1, 3);
        animator.SetTrigger("Dance_" + randomNumber);
    }
}
