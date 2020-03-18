using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Удар мечём. Фейковый.
/// Фейк - так как это просто круг с анимированый. Он посещает места удара мечом.
/// </summary>
public class SwordWeapon : MonoBehaviour
{

    [Header("Урон от атаки мечом")]
    [SerializeField]
    private float damage = 7;
    
    //Защита от двойного удара. Бывает когда колайдер 2 раза входит в противника.
    private bool isDamage = false;
    private float damageTimer = 0.0f;

    private int findIndex;



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

            findIndex = GameManager.Instance.enemyInRoom[GameManager.Instance.camPointNumber].enemyPWGO.IndexOf(collision.gameObject);
            GameManager.Instance.enemyInRoom[GameManager.Instance.camPointNumber].enemyPW[findIndex].TakeDamage(Random.Range((damage - (damage / 5)), (damage + (damage / 5))));

        }

    }

}
