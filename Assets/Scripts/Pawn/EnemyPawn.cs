using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : Pawn
{

    protected override void Awake()
    {
        
        base.Awake();

        GameManager.Instance.EnemyPawn.Add(this);
        GameManager.Instance.EnemyPawnTransform.Add(m_Transform);

    }

    protected override void Update()
    {

        base.Update();

    }

    public void Attack()
    {

        m_Animator.SetTrigger("Attack");

    }

    protected override void Die()
    {

        base.Die();

    }

}
