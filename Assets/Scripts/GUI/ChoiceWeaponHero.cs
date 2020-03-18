using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт выбора оружия. Вешаем на кнопку и выбираем нужное оружее.
/// </summary>
[RequireComponent(typeof(Button))]
public class ChoiceWeaponHero : MonoBehaviour
{

    [Header("Выберите оружее:")]
    [SerializeField]
    private GameManager.WeaponHero m_WeaponHero;

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
