using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MonoBehaviour
{

    public float damage = 7;

    private GameObject hitGO;

    private bool isDamage = false;
    private float damageTimer = 0.0f;


    private void Awake()
    {
        

    }

    protected virtual void Update()
    {

        if (isDamage) damageTimer += Time.deltaTime;
        if (damageTimer > 1.0f)
        {

            isDamage = false;
            damageTimer = 0.0f;

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy") && !isDamage)
        {

            isDamage = true;
            collision.gameObject.GetComponent<EnemyPawn>().TakeDamage(Random.Range((damage - (damage / 5)), (damage + (damage / 5))));
            
        }

    }

}
