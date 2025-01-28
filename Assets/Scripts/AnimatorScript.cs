using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RollDice()
    {
        animator.SetBool("isRolling", true);
    }

    public void StopRoll()
    {
        animator.SetBool("isRolling", false);
    }
}
