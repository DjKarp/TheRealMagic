using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : Pawn
{

    private GameObject hitGO;
    private Animator hitAnimator;

    public float damage = 7.0f;

    private bool isAttack = false;

    protected override void Awake()
    {
        
        base.Awake();
        
        GameManager.Instance.changeGameModeEvent += OnGameChangeState;
        
    }

    protected override void Update()
    {

        base.Update();

    }

    public void Attack()
    {

        isAttack = true;

        m_Animator.SetTrigger("Attack");

        hitGO = Instantiate(GameManager.Instance.hitPrefab, GameManager.Instance.m_HeroTransform.position, Quaternion.identity);
        hitAnimator = hitGO.GetComponent<Animator>();
        hitAnimator.SetTrigger("Start");

        GameManager.Instance.m_HeroPawn.TakeDamage(damage);

    }

    protected override void Die()
    {

        base.Die();

        GameManager.Instance.enemyInRoom[GameManager.Instance.camPointNumber].enemyGO.Remove(gameObject);
        GameManager.Instance.enemyLiveInScene--;

    }

    public void OnGameChangeState()
    {
        /*
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.EnemyTurn)
        {

            if (HP > 0) StartCoroutine(EnemyTurn());

        }
        */
    }

    IEnumerator EnemyTurn()
    {

        isAttack = false;

        yield return new WaitForSeconds(2.0f);

        if (!isAttack) Attack();

        GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyWeaponWait);

        yield return new WaitForSeconds(1.0f);

        if (!GameManager.Instance.m_HeroPawn.IsDie()) GameManager.Instance.ChangeGameMode(GameManager.GameMode.PlayerTurn);

        yield break;

    }

}
