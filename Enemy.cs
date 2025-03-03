using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnHitReaction(string type)
    {
        animator.ResetTrigger("HeadHit");
        animator.ResetTrigger("KickHit");
        animator.ResetTrigger("PunchHit");

        switch (type)
        {
            case "FlipKick":
                animator.SetTrigger("KickHit");
                break;
            case "CrossPunch":
                animator.SetTrigger("PunchHit");
                break;
            case "FlyingKick":
                animator.SetTrigger("HeadHit");
                break;
            case "FlyingPunch":
                animator.SetTrigger("PunchHit");
                break;
            default:
                animator.SetTrigger("PunchHit");
                break;
        }
    }
}
