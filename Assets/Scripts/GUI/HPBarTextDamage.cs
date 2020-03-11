using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPBarTextDamage : MonoBehaviour
{

    private Animator m_Animator;
    private TextMeshPro m_TextMeshPro;

    private void Awake()
    {

        m_Animator = gameObject.GetComponent<Animator>();

        m_TextMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();

    }

    public void TakeDamage(float damage)
    {

        m_TextMeshPro.text = "- " + Mathf.RoundToInt(damage).ToString();

        m_Animator.SetTrigger("isStart");

    }

    public void TakeHealth(float addHPCount)
    {

        m_TextMeshPro.text = "+ " + Mathf.RoundToInt(addHPCount).ToString();

        m_Animator.SetTrigger("isStart");

    }

}
