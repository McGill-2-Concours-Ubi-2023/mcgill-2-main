using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccludableWall : MonoBehaviour
{
    private Animator animator;
    private Material material;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        material = GetComponent<Renderer>().material;
    }

    public void Hide()
    {
        if (animator != null)
        {
            animator.SetBool("Hide", true);
            animator.SetBool("Occlude", false);
        }        
    }

    public void Occlude()
    {
        if (animator != null) 
        {
            animator.SetBool("Hide", false);
            animator.SetBool("Occlude", true);
        }
        
    }

    public void ChangeRenderQueue(int queue)
    {
        material.renderQueue = queue;
    }
}
