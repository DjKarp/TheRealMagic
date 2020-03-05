using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//isCastMagic
//isWalking
//isWinner
public class HeroPawn : Pawn
{

    protected override void Awake()
    {

        base.Awake();

    }

    protected override void Update()
    {

        base.Update();

    }

    public override void TakeDamage(float damage)
    {
        
        base.TakeDamage(damage);


    }

    protected override void Die()
    {

        base.Die();

    }

    public void HeroMove(bool isMoveHeroNow)
    {

        m_Animator.SetBool("isWalking", isMoveHeroNow);

    }

}
