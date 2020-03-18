using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Наследованый павн врагов в игре 
/// </summary>
public class EnemyPawn : Pawn
{
    //FX получения урона, его аниматор и трансформ. 
    private GameObject hitGOPrefab;
    private GameObject hitGO;
    private Transform hitGOTransform;
    private Animator hitAnimator;

    [Header("Наносимый герою урон:")]
    public float damage = 7.0f;

    //Дополнительное условие, чтобы мобы не аттаковали несколько раз за ход. 
    private bool isAttack = false;

    private int findIndex;



    protected override void Awake()
    {
        
        base.Awake();

        //подгружаем FX попадания, чтобы был в памяти.
        hitGOPrefab = Resources.Load("Hit") as GameObject;
        hitGO = Instantiate(hitGOPrefab);
        hitGOTransform = hitGO.GetComponent<Transform>();
        hitGOTransform.parent = m_Transform.parent;
        hitAnimator = hitGO.GetComponent<Animator>();

    }

    public void Attack()
    {

        isAttack = true;

        m_Animator.SetTrigger("Attack");

        hitGOTransform.position = GameManager.Instance.m_HeroTransform.position;
        hitAnimator.SetTrigger("isStart");

        //Зандомизация урона - начальный дамаг +\- треть от него.
        GameManager.Instance.m_HeroPawn.TakeDamage(Random.Range((damage - (damage/3)), (damage + (damage/3))));

    }

    protected override void Die()
    {

        GameManager.Instance.enemyInRoom[GameManager.Instance.camPointNumber].enemyGO.Remove(m_Transform.parent.gameObject);

        base.Die();
        
    }
    
    IEnumerator EnemyTurn()
    {

        isAttack = false;

        yield return new WaitForSeconds(0.5f);

        if (!isAttack) Attack();

        GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyWeaponWait);

        yield return new WaitForSeconds(0.5f);

        if (!GameManager.Instance.m_HeroPawn.IsDie()) GameManager.Instance.ChangeGameMode(GameManager.GameMode.PlayerTurn);

        yield break;

    }

}
