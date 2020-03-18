using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Анимированное отображение полученого урона \ лечения
/// Вешается на сам компонент с аниматором
/// </summary>
[RequireComponent(typeof(Animator))]
public class HPBarTextDamage : MonoBehaviour
{

    private Animator m_Animator;
    private TextMeshPro m_TextMeshPro;
    private string prefixDamage = "- ";
    private string prefixHealth = "+ ";

    private void Awake()
    {

        m_Animator = gameObject.GetComponent<Animator>();
        m_TextMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();

    }

    public void TakeDamage(float damage)
    {

        ShowChangeHealth(prefixDamage, damage);

    }

    public void TakeHealth(float addHPCount)
    {

        ShowChangeHealth(prefixHealth, addHPCount);

    }

    private void ShowChangeHealth(string prefix, float value)
    {

        m_TextMeshPro.text = prefix + Mathf.RoundToInt(value).ToString();
        m_Animator.SetTrigger("isStart");

    }

}
