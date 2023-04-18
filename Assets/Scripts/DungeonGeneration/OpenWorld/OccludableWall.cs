using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccludableWall : MonoBehaviour
{
    public Animator animator;
    private Material material;

    private void Awake()
    {
        animator = transform.parent.GetComponentInParent<Animator>();
        material = GetComponent<Renderer>().material;
    }

    public void Hide()
    {
        //Debug.Log("HIDE");
        if (animator != null)
        {
            animator.SetBool("Hide", true);
            animator.SetBool("Occlude", false);
        }        
    }

    public void Occlude()
    {
        //Debug.Log("SHOW");
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
