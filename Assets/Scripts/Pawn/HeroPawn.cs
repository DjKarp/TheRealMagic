using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Наследованый павн игрока в игре 
/// </summary>
//isCastMagic
//isWalking
//isWinner
public class HeroPawn : Pawn
{

    [Header("Скорость передвижения персонажа по уровню:")]
    [SerializeField]
    private float speed = 15.0f;

    //Координаты по Х правой и левой крайних точек экрана
    //Для проверки выхода игрока за экран
    private float rightEdge;
    private float leftEdge;

    private SpriteRenderer m_SpriteRenderer;

    //Колайдер фейковой атаки меча. 
    public CircleCollider2D fakeSwordCollider;

    //Количество очков лечения при поднятии хилки
    private float poisen = 50.0f;


    protected override void Awake()
    {

        base.Awake();
        
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                
    }

    protected override void Update()
    {

        base.Update();

        MoveHero();

    }

    public override void TakeDamage(float damage)
    {
        
        if (!isDamage)
        {

            isDamage = true;

            HP = Mathf.Clamp(HP - (damage * GameManager.Instance.levelOfComplexity), 0.0f, maxHP);

            m_Animator.SetTrigger("Damage");

            m_HPBarTextDamage.TakeDamage(damage);

            CheckDie();

        }

    }


    public  void TakePoisen()
    {
        
        HP = Mathf.Clamp(HP + poisen, 0.0f, maxHP);

        m_Animator.SetTrigger("isPoisen");

        m_HPBarTextDamage.TakeHealth(poisen);

        CheckDie();

    }

    protected override void Die()
    {

        base.Die();

        GameManager.Instance.ChangeGameMode(GameManager.GameMode.Loose);

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
    
    private void MoveHero()
    {

        if(GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerTurn && GameManager.Instance.camPointNumber > 0)
        {
            
            if (Input.GetAxis("Horizontal") > 0 & Input.anyKey)
            {

                if (!m_Animator.GetBool("isWalking")) HeroMove(true);
                if (m_SpriteRenderer.flipX) m_SpriteRenderer.flipX = false;
                m_Transform.position = new Vector3(m_Transform.position.x + ((speed / 7) * Time.deltaTime), m_Transform.position.y, m_Transform.position.z);

            }
            else if (Input.GetAxis("Horizontal") < 0 & Input.anyKey)
            {

                if (!m_Animator.GetBool("isWalking")) HeroMove(true);
                if (!m_SpriteRenderer.flipX) m_SpriteRenderer.flipX = true;
                m_Transform.position = new Vector3(m_Transform.position.x - ((speed / 7) * Time.deltaTime), m_Transform.position.y, m_Transform.position.z);

            }
            else if (!Input.anyKey)
            {

                if (m_Animator.GetBool("isWalking")) HeroMove(false);

            }

            //Проверка на выход игрока за пределы камеры
            if (Input.GetAxis("Horizontal") != 0)
            {
                
                rightEdge = GameManager.Instance.CameraTransform().position.x + 8.5f;
                leftEdge = GameManager.Instance.CameraTransform().position.x - 8.5f;

                if (m_Transform.position.x > rightEdge) m_Transform.position = new Vector3(rightEdge, m_Transform.position.y, m_Transform.position.z);
                else if (m_Transform.position.x < leftEdge) m_Transform.position = new Vector3(leftEdge, m_Transform.position.y, m_Transform.position.z);

            }
            
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Healt"))            
        {

            TakePoisen();
            collision.gameObject.SetActive(false);

        }
        else if (collision.gameObject.CompareTag("DestructionObject"))
        {

            if (m_Animator.GetBool("isWalking")) HeroMove(false);

        }

    }
    
}
