using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBallWeapon : MonoBehaviour
{

    public int collisionCount = 0;
    public int maxCollisionCount = 2;

    public bool isTheEnd = false;

    public float damage = 7;

    private GameObject hitGO;
    private Animator hitAnimator;

    [SerializeField]
    private GameObject explousenPrefab;

    private Rigidbody2D m_Rigidbody2D;

    private void Start()
    {

        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

    }

    private void Update()
    {

        if (!m_Rigidbody2D.isKinematic && collisionCount > 0 && m_Rigidbody2D.velocity.x <= 0.0f && m_Rigidbody2D.velocity.y <= 0.0f)
        {

            isTheEnd = true;
            Instantiate(explousenPrefab, gameObject.transform.position, Quaternion.identity);
            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
            Destroy(gameObject);

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall"))
        {

            collisionCount++;

            if (collisionCount > maxCollisionCount)
            {

                isTheEnd = true;
                Instantiate(explousenPrefab, collision.GetContact(0).point, Quaternion.identity);
                GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
                Destroy(gameObject);

            }

        }
        else if (collision.gameObject.CompareTag("Enemy") | collision.gameObject.CompareTag("Player"))
        {

            isTheEnd = true;
            collision.gameObject.GetComponent<Pawn>().TakeDamage(Random.Range((damage - (damage / 5)), (damage + (damage / 5))));

            hitGO = Instantiate(explousenPrefab, collision.GetContact(0).point, collision.transform.rotation);

            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);

            Destroy(gameObject);

        }
        else if (collision.gameObject.CompareTag("DestructionObject"))
        {

            collisionCount++;

            hitGO = Instantiate(explousenPrefab, collision.GetContact(0).point, collision.transform.rotation);
            hitGO.transform.parent = null;
            Destroy(collision.gameObject);

            if (collisionCount > maxCollisionCount)
            {

                isTheEnd = true;
                Instantiate(explousenPrefab, collision.GetContact(0).point, Quaternion.identity);
                GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
                Destroy(gameObject);

            }

        }

    }

}
