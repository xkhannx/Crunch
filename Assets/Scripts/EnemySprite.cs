using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Death()
    {
        animator.SetBool("eating", false);
        animator.SetTrigger("die");
    }

    public void Rebirth()
    {
        animator.SetTrigger("reborn");
    }

    public void Eat(bool eat)
    {
        animator.SetBool("eating", eat);
    }

    // Animation event
    public void DeathComplete()
    {
        GetComponentInParent<Enemy>().readyToRespawn = true;
    }
}
