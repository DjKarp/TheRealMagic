using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//isCastMagic
//isWalking
//isWinner
public class HeroPawn : Pawn
{

    public float swordDamage = 3.0f;

    private EnemyPawn m_EnemyPawn;

    private BoxCollider2D m_BoxCollider2D;    
    private CapsuleCollider2D m_CapsuleCollider2D;


    protected override void Awake()
    {

        base.Awake();

        m_BoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        m_CapsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();

        isAttackSwordSwitchCollider(false);

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
        
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerWeaponWait | 
            (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerTurn & GameManager.Instance.CurrentWeaponHero == GameManager.WeaponHero.Sword))
        {

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Knight_attack"))
            {

                m_EnemyPawn = collision.gameObject.GetComponent<EnemyPawn>();
                if (m_EnemyPawn != null) m_EnemyPawn.TakeDamage(swordDamage);

            }

        }

    }

    public void isAttackSwordSwitchCollider(bool isStart)
    {

        m_BoxCollider2D.enabled = !isStart;
        m_CapsuleCollider2D.enabled = isStart;

    }

}
