using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Основа для всех персонажей. Общие переменные и методы.
/// </summary>
[RequireComponent(typeof(Animator))]
public class Pawn : MonoBehaviour
{

    //Трансформ, аниматор самого Pawn
    protected Transform m_Transform;
    protected Animator m_Animator;

    //Текущий и максимальный уровни здоровья
    public float HP;
    public float maxHP;

    //Скрипт сам выставляет уровень здоровья на полоске HP Bar.
    //И так как сам бар делается как спрайт на сцене (а не GUI элемент), то изменять размер полоски будем скейлом по оси. 
    //Для этого надо не забывать, что пивот полоски должен быть в нуле, т.е. в самой левой центральной части, если полоска уменьшается справа на лево.
    [Header("Sprite самой полоски HP Bar'а.")]
    [SerializeField]
    protected Transform hpBar;
    //Запоминаем начальный скейл полоски. Так как он может и не равняться еденице. Тогда будем апромиксировать.
    protected float startHpBarValue;   

    //Точка из которой снаряд стартует.
    public Transform shootPoint;

    //Для вызова всплывающей анимации изменения солстояния здоровья павна.
    public HPBarTextDamage m_HPBarTextDamage;

    //Проверка на то, чтобы дамаг не проходил несколько раз от одного оружия за один ход.
    public bool isDamage = false;
    public float damageTimer = 0.0f;


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
