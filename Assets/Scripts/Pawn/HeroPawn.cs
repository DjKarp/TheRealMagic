using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//isCastMagic
//isWalking
//isWinner
public class HeroPawn : Pawn
{

    public float swordDamage = 3.0f;


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

    public void AttackSwordHero()
    {

        m_Animator.SetTrigger("SwordAttack");

    }

    public void AttackMagicHero(bool startEnd)
    {

        m_Animator.SetBool("isCastMagic", startEnd);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerWeaponWait)
        {

            collision.gameObject.GetComponent<EnemyPawn>().TakeDamage(swordDamage);

        }

    }

}
