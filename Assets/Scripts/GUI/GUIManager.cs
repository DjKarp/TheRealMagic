using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GUIManager : MonoBehaviour
{

    public static GUIManager Instance { get; private set; } = null;

    [SerializeField]
    private GameObject m_DialogueWindow;




    private void Awake()
    {

        SearchDestroyCopySingletonOrThisCreateInstance();

        ShowAndHideDialogWindow(false, 0);

    }






    public void ShowAndHideDialogWindow(bool isShow, int numberText)
    {

        m_DialogueWindow.SetActive(isShow);

    }

    private void SearchDestroyCopySingletonOrThisCreateInstance()
    {

        if (Instance)
        {

            DestroyImmediate(gameObject);
            return;

        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

    }

}
