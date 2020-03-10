using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


//isCastMagic
//isWalking
//isWinner
public class HeroPawn : Pawn
{

    public float swordDamage = 3.0f;

    public float speed = 10.0f;
    private bool isMove = false;

    private SpriteRenderer m_SpriteRenderer;

    private EnemyPawn m_EnemyPawn;
    

    protected override void Awake()
    {

        base.Awake();

        GameManager.Instance.changeGameModeEvent += OnGameChangeState;

        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
    }

    protected override void Update()
    {

        base.Update();

        //MoveHero();

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

    public void OnGameChangeState()
    {

        switch (GameManager.Instance.CurrentGameMode)
        {

            case GameManager.GameMode.PlayerTurn:
                //StartCoroutine(MoveHero());
                break;

        }

    }

    private void MoveHero()
    {

        while(GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerTurn)
        {

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {

                if (!isMove)
                {

                    HeroMove(true);
                    isMove = true;

                }

                if (m_SpriteRenderer.flipX) m_SpriteRenderer.flipX = false;

                //m_Transform.position = new Vector3(m_Transform.position.x + (speed * Time.deltaTime), m_Transform.position.y, m_Transform.position.z);

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {

                HeroMove(true);

                if (!m_SpriteRenderer.flipX) m_SpriteRenderer.flipX = true;

                m_Transform.Translate(Vector3.left * speed * Time.deltaTime);

            }
            else
            {

                if (isMove)
                {

                    HeroMove(false);
                    isMove = false;

                }

            }

        }

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
    
}
