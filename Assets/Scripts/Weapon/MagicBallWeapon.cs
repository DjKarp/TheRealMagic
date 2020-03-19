using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт отвечающий за попадания магическими шарами
/// </summary>
public class MagicBallWeapon : Weapon
{
    
    private Rigidbody2D m_Rigidbody2D;

    public override void Awake()
    {

        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

        base.Awake();
        
    }

    public override void LoadResources()
    {

        hitGOPrefab = Resources.Load("Hit") as GameObject;

        if (hitSimplePrefab == null) hitSimplePrefab = Resources.Load("FX_Explosion") as GameObject;


    }

    private void Update()
    {

        //Если остановился, то взрывается.
        if (!m_Rigidbody2D.isKinematic && collisionCount > 0 && m_Rigidbody2D.velocity.x <= 0.0f && m_Rigidbody2D.velocity.y <= 0.0f)
        {

            if (m_Transform != null && hitSimpleTransform != null) hitSimpleTransform.position = m_Transform.position;
            if (hitSimplePS != null) hitSimplePS.Play();
            if (hitSimpleAnimator != null) hitSimpleAnimator.SetTrigger("isStart");

            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
            
            gameObject.SetActive(false);

        }

    }
    
}
