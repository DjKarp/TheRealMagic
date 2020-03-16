using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StateButtonAndDiffycly : MonoBehaviour
{

    [Header("Кнопка.")]
    [SerializeField]
    private Button m_Button;

    public Difficly m_Difficly = Difficly.Normal;
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
        
    }
}
