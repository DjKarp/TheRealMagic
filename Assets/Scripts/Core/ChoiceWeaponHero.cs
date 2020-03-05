using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChoiceWeaponHero : MonoBehaviour
{

    public GameManager.WeaponHero m_WeaponHero;

    private Button m_Button;

    private void Awake()
    {

        if (m_Button == null) m_Button = gameObject.GetComponent<Button>();

        m_Button.onClick.AddListener(TaskOnClick);

    }

    void TaskOnClick()
    {

        GameManager.Instance.ChangeWeaponHero(m_WeaponHero);

    }

}
