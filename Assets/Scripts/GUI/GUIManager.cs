using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public static GUIManager Instance { get; private set; } = null;

    [SerializeField]
    private GameObject[] m_DialogueWindow;

    [SerializeField]
    private GameObject m_WeaponChoice;
    [SerializeField]
    private GameObject[] m_WeaponOff;
    [SerializeField]
    private Transform[] weaponButtonTransform;

    [SerializeField]
    private GameObject powerArrowGO;
    private Transform powerArrowTR;
    [SerializeField]
    private Image powerArrowImage;

    [SerializeField]
    private GameObject TextCurrentTurn;
    private TextMeshProUGUI m_TextMesh;

    private Vector3 xDirection;
    private Vector3 yDirection;
    private Vector3 zDirection;


    private void Awake()
    {

        SearchDestroyCopySingletonOrThisCreateInstance();

        for(int i = 0; i < m_DialogueWindow.Length; i++) ShowAndHideDialogWindow(false, i);
        ShowAndHideWeaponChoice(false);
        ShowAndHidePowerArrow(false);

        for (int i = 0; i < m_WeaponOff.Length; i++) if (i < GameManager.Instance.openWeapon) m_WeaponOff[i].SetActive(false);

        m_TextMesh = TextCurrentTurn.GetComponent<TextMeshProUGUI>();
        if (m_TextMesh == null) m_TextMesh = TextCurrentTurn.GetComponentInChildren<TextMeshProUGUI>();
        TextCurrentTurn.SetActive(false);

        GameManager.Instance.changeGameModeEvent += OnChangeState;

        powerArrowTR = powerArrowGO.transform;

    }


    public void OnChangeState()
    {

        switch (GameManager.Instance.CurrentGameMode)
        {

            case GameManager.GameMode.PlayerTurn:
                ShowTextCurrentTurn("ход игрока", 0);
                break;

            case GameManager.GameMode.EnemyTurn:
                ShowTextCurrentTurn("ход врагов", 0);
                break;

            default:
                ShowAndHideWeaponChoice(false);
                StartCoroutine(HideTextTurn(0.0f));
                break;

        }

    }

    private void Update()
    {
        
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerTurn)
        {

            powerArrowTR.position = Camera.main.WorldToScreenPoint(GameManager.Instance.m_HeroPawn.shootPoint.position);
            powerArrowTR.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.Angle(Vector2.right, (Input.mousePosition - powerArrowTR.position)));

        } 

    }

    public void OnWeaponOff()
    {

        for (int i = 0; i < m_WeaponOff.Length; i++)
        {

            if (GameManager.Instance.openWeapon == i)
            {

                m_WeaponOff[i].SetActive(false);

                weaponButtonTransform[i].DOShakeScale(1.4f);

                GameManager.Instance.SetWeaponOffValue();

                break;

            }

        }

    }

    public void ShowAndHideDialogWindow(bool isShow, int numberText)
    {

        if (m_DialogueWindow[numberText].activeSelf != isShow) m_DialogueWindow[numberText].SetActive(isShow);

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

    public void ShowTextCurrentTurn(string m_Text, int numberEnemyTurn)
    {

        TextCurrentTurn.SetActive(true);
        if (numberEnemyTurn == 0) m_TextMesh.text = m_Text;
        else m_TextMesh.text = m_Text + numberEnemyTurn;

    }

    IEnumerator HideTextTurn(float hideTime)
    {

        yield return new WaitForSeconds(hideTime);

        TextCurrentTurn.SetActive(false);

        yield break;

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

    public void LookAt2D(Vector3 lookTarget, Transform rotateTransform)
    {

        //Желаемое направление оси X, от которого устанавливаем ось Y. Z ось обращена к нам.
        xDirection = (lookTarget - transform.position).normalized;
        yDirection = Quaternion.Euler(0, 0, 90) * xDirection;
        zDirection = Vector3.forward;

        rotateTransform.rotation = Quaternion.LookRotation(zDirection, yDirection);

    }

}
