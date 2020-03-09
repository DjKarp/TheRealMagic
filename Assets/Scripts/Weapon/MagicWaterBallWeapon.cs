using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWaterBallWeapon : MonoBehaviour
{

    public bool isTheEnd = false;

    public float damage = 4;

    private GameObject hitGO;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall"))
        {

            isTheEnd = true;
            hitGO = Instantiate(GameManager.Instance.hitWaterBallExplousenPrefab, collision.GetContact(0).point, collision.transform.rotation);
            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
            Destroy(gameObject);

        }
        else if (collision.gameObject.CompareTag("Enemy") | collision.gameObject.CompareTag("Player"))
        {

            isTheEnd = true;
            collision.gameObject.GetComponent<Pawn>().TakeDamage(damage);

            hitGO = Instantiate(GameManager.Instance.hitWaterBallExplousenPrefab, collision.GetContact(0).point, collision.transform.rotation);

            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);

            Destroy(gameObject);

        }
        else if (collision.gameObject.CompareTag("DestructionObject"))
        {

            isTheEnd = true;
            hitGO = Instantiate(GameManager.Instance.hitWaterBallExplousenPrefab, collision.GetContact(0).point, collision.transform.rotation);
            hitGO.transform.parent = null;
            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);
            Destroy(collision.gameObject);
            Destroy(gameObject);

        }

    }

}
