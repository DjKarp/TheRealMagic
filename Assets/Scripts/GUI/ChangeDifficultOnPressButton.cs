using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Установка уровня сложности по нажатию на кнопку.
/// </summary>
[RequireComponent(typeof(Button))]
public class ChangeDifficultOnPressButton : MonoBehaviour
{

    [Header("Кнопка.")]
    [SerializeField]
    private Button m_Button;

    [Header("В какой игровой режим переводит кнопка.")]
    [SerializeField]
    private Difficly m_Difficly = Difficly.Normal;

    public enum Difficly
    {

        Hard,
        Normal,
        Easy

    }
    
    private MainMenuController m_MainMenuController;


    private void Awake()
    {

        if (m_Button == null) m_Button = gameObject.GetComponent<Button>();

        m_MainMenuController = FindObjectOfType<MainMenuController>();

        m_Button.onClick.AddListener(TaskOnClick);
        
    }
    
    public void TaskOnClick()
    {              

        switch (m_Difficly)
        {

            case Difficly.Hard:
                PlayerPrefs.SetFloat("LevelOfComplexity", 1.0f);
                break;

            case Difficly.Normal:
                PlayerPrefs.SetFloat("LevelOfComplexity", 0.75f);
                break;

            case Difficly.Easy:
                PlayerPrefs.SetFloat("LevelOfComplexity", 0.5f);
                break;

        }

        m_MainMenuController.SetDiffyclyEnd();

        SoundAndMusic.Instance.ChangePlayingMusic(1);
        
    }
}
