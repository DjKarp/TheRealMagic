using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightingWeapon : MonoBehaviour
{
    
    private Transform m_Transform;
    private Rigidbody2D m_Rigidbody2D;

    public int collisionCount = 0;
    public int maxCollisionCount = 4;

    public bool isTheEnd = false;

    public float damage = 4;

    private GameObject hitGO;
    private Animator hitAnimator;
    
    //для преобразования при повороте
    private Vector3 xDirection;
    private Vector3 yDirection;
    private Vector3 zDirection;

    private void Awake()
    {

        m_Transform = gameObject.transform;
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();        

    }

    private void Update()
    {
        
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerTurn) LookAt2D(GameManager.Instance.GetOurMouseMosition());
        else if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerWeaponWait) LookAt2D(m_Rigidbody2D.velocity);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall"))
        {

            collisionCount++;

            if (collisionCount > maxCollisionCount)
            {

                isTheEnd = true;
                hitGO = Instantiate(GameManager.Instance.hitSimpleLightingPrefab, collision.GetContact(0).point, Quaternion.identity);
                GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
                Destroy(gameObject);

            }

        }
        else if (collision.gameObject.CompareTag("Enemy") | collision.gameObject.CompareTag("Player"))
        {

            isTheEnd = true;
            collision.gameObject.GetComponent<EnemyPawn>().TakeDamage(damage);

            hitGO = Instantiate(GameManager.Instance.hitPrefab, collision.GetContact(0).point, Quaternion.identity);
            hitAnimator = hitGO.GetComponent<Animator>();
            hitAnimator.SetTrigger("Start");

            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);

            Destroy(gameObject);

        }
        else if (collision.gameObject.CompareTag("DestructionObject"))
        {

            collisionCount++;

            if (collisionCount > maxCollisionCount)
            {

                isTheEnd = true;
                hitGO = Instantiate(GameManager.Instance.hitSimpleLightingPrefab, collision.GetContact(0).point, Quaternion.identity);
                GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
                Destroy(gameObject);

            }
            else
            {

                hitGO = Instantiate(GameManager.Instance.hitRockDestroyPrefab, collision.GetContact(0).point, Quaternion.identity);
                hitGO.transform.parent = null;
                Destroy(collision.gameObject);

            }

        }

    }

    private void OnEnable()
    {

        collisionCount = 0;
        isTheEnd = false;

    }

    public void LookAt2D(Vector3 lookTarget)
    {

        //Желаемое направление оси X, от которого устанавливаем ось Y. Z ось обращена к нам.
        xDirection = (lookTarget - transform.position).normalized;
        yDirection = Quaternion.Euler(0, 0, 90) * xDirection;
        zDirection = Vector3.forward;

        m_Transform.rotation = Quaternion.LookRotation(zDirection, yDirection);

    }

}

