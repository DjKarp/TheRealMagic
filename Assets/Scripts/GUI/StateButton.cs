using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StateButton : MonoBehaviour
{

    [Header("Кнопка.")]
    [SerializeField]
    private Button m_Button;

    [Header("В какой игровой режим переводит кнопка.")]
    [SerializeField]
    private GameManager.GameMode new_GameMode;


    private void Awake()
    {

        if (m_Button == null) m_Button = gameObject.GetComponent<Button>();

        m_Button.onClick.AddListener(TaskOnClick);

    }
    
    void TaskOnClick()
    {

        GameManager.Instance.ChangeGameMode(new_GameMode);

    }
}
