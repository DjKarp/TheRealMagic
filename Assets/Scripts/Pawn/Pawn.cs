using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{

    protected Transform m_Transform;

    public float HP;
    public float maxHP;

    [Header("Sprite самой полоски HP Bar'а.")]
    [SerializeField]
    protected Transform hpBar;
    protected float startHpBarValue;

    protected Animator m_Animator;

    public Transform shootPoint;

    public HPBarTextDamage m_HPBarTextDamage;

    private bool isDamage = false;
    private float damageTimer = 0.0f;


    protected virtual void Awake()
    {

        HP = maxHP;

        m_Transform = gameObject.transform;
        startHpBarValue = hpBar.localScale.x;

        m_Animator = gameObject.GetComponent<Animator>();
        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();

        m_HPBarTextDamage = m_Transform.parent.gameObject.GetComponentInChildren<HPBarTextDamage>();
        
    }

    protected virtual void Update()
    {

        if (isDamage) damageTimer += Time.deltaTime;
        if (damageTimer > 5.0f)
        {

            isDamage = false;
            damageTimer = 0.0f;

        }

    }

    public virtual void TakeDamage(float damage)
    {

        if (!isDamage)
        {

            isDamage = true;

            HP = Mathf.Clamp(HP - (damage / GameManager.Instance.levelOfComplexity), 0.0f, maxHP);

            m_Animator.SetTrigger("Damage");

            m_HPBarTextDamage.TakeDamage(damage);

            CheckDie();

        }       

    }

    protected virtual void CheckDie()
    {

        if (HP <= 0.0f)
        {

            hpBar.localScale = new Vector3(0.0f, hpBar.localScale.y, hpBar.localScale.z);

            Die();

        }
        else
        {

            hpBar.localScale = new Vector3((startHpBarValue * HP) / maxHP, hpBar.localScale.y, hpBar.localScale.z);

        }

    }

    public virtual bool IsDie()
    {

        if (HP <= 0.0f) return true;
        else return false;

    }

    protected virtual void Die()
    {
        
        m_Animator.SetTrigger("Die");

        Destroy(m_Transform.parent.gameObject, 2.0f);

    }    

}
