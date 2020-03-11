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

    [Header("В какой игровой режим переводит кнопка.")]
    [SerializeField]
    private GameManager.GameMode new_GameMode;

    public Difficly m_Difficly;
    public enum Difficly
    {

        Hard,
        Normal,
        Easy

    }

    private GameObject parentGO;


    private void Awake()
    {

        if (m_Button == null) m_Button = gameObject.GetComponent<Button>();

        m_Button.onClick.AddListener(TaskOnClick);

        parentGO = gameObject.transform.parent.gameObject;

    }
    
    void TaskOnClick()
    {              

        switch (m_Difficly)
        {

            case Difficly.Hard:
                GameManager.Instance.levelOfComplexity = 1.0f;
                break;

            case Difficly.Normal:
                GameManager.Instance.levelOfComplexity = 0.75f;
                break;

            case Difficly.Easy:
                GameManager.Instance.levelOfComplexity = 0.5f;
                break;

        }

        GameManager.Instance.m_Camera.transform.position = GameManager.Instance.camPathPoint[0].position;

        if (GameManager.Instance.CurrentGameMode != new_GameMode) GameManager.Instance.ChangeGameMode(new_GameMode);

        parentGO.SetActive(false);

    }
}
