using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscendMessage : MonoBehaviour
{
    private Animator animator;

    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("ShowMessage");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("HideMessage");
        }
    }
}
