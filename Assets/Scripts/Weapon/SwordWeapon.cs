using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MonoBehaviour
{

    public float damage = 7;

    private GameObject hitGO;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {

            collision.gameObject.GetComponent<EnemyPawn>().TakeDamage(damage);
            
        }

    }

}
