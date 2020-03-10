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
    private bool isLeft = false;
    private bool isRight = false;
    private float rightEdge;
    private float leftEdge;

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

        MoveHero();

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

        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.PlayerTurn) if (m_SpriteRenderer.flipX) m_SpriteRenderer.flipX = false;

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

        if(GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerTurn)
        {
            
            if (Input.GetKeyDown(KeyCode.RightArrow) && !isRight)
            {

                if (!isMove)
                {

                    HeroMove(true);
                    isMove = true;

                }

                if (m_SpriteRenderer.flipX) m_SpriteRenderer.flipX = false;

                isRight = true;

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && !isLeft)
            {

                if (!isMove)
                {

                    HeroMove(true);
                    isMove = true;

                }

                if (!m_SpriteRenderer.flipX) m_SpriteRenderer.flipX = true;

                isLeft = true;

            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) | Input.GetKeyUp(KeyCode.LeftArrow))
            {

                if (isMove)
                {

                    HeroMove(false);
                    isMove = false;
                    isRight = false;
                    isLeft = false;

                }

            }

            if (isMove)
            {

                if (isRight) m_Transform.position = new Vector3(m_Transform.position.x + ((speed / 7) * Time.deltaTime), m_Transform.position.y, m_Transform.position.z);
                if (isLeft) m_Transform.position = new Vector3(m_Transform.position.x - ((speed / 7) * Time.deltaTime), m_Transform.position.y, m_Transform.position.z);

                rightEdge = GameManager.Instance.CameraTransform().position.x + 8.5f;
                leftEdge = GameManager.Instance.CameraTransform().position.x - 8.5f;

                if (m_Transform.position.x > rightEdge) m_Transform.position = new Vector3(rightEdge, m_Transform.position.y, m_Transform.position.z);
                else if (m_Transform.position.x < leftEdge) m_Transform.position = new Vector3(leftEdge, m_Transform.position.y, m_Transform.position.z);

            }
            
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        

    }
    
}
