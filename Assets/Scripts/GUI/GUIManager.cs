using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GUIManager : MonoBehaviour
{

    public static GUIManager Instance { get; private set; } = null;

    [SerializeField]
    private GameObject m_DialogueWindow;

    [SerializeField]
    private GameObject m_WeaponChoice;

    [SerializeField]
    private GameObject[] m_WeaponOff;

    [SerializeField]
    private Transform[] weaponButtonTransform;

    [SerializeField]
    private GameObject powerArrowGO;

    [SerializeField]
    private Image powerArrowImage;




    private void Awake()
    {

        SearchDestroyCopySingletonOrThisCreateInstance();

        ShowAndHideDialogWindow(false, 0);
        ShowAndHideWeaponChoice(false);
        ShowAndHidePowerArrow(false);

        for (int i = 0; i < m_WeaponOff.Length; i++) if (i < GameManager.Instance.openWeapon) m_WeaponOff[i].SetActive(false);

    }




    public void OnWeaponOff()
    {

        for (int i = 0; i < m_WeaponOff.Length; i++)
        {

            if (GameManager.Instance.openWeapon == i)
            {

                m_WeaponOff[i].SetActive(false);

                weaponButtonTransform[i].DOShakeScale(1.4f);
                weaponButtonTransform[i].DOShakeRotation(1.4f);

                GameManager.Instance.SetWeaponOffValue();

                break;

            }

        }

    }

    public void ShowAndHideDialogWindow(bool isShow, int numberText)
    {

        m_DialogueWindow.SetActive(isShow);

    }

    public void ShowAndHideWeaponChoice(bool isShow)
    {

        m_WeaponChoice.SetActive(isShow);

    }

    public void ShowAndHidePowerArrow(bool isShow)
    {

        powerArrowGO.SetActive(isShow);

    }
    public void SetPowerArrowSliderValue(float m_Value)
    {

        powerArrowImage.fillAmount = m_Value;

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
