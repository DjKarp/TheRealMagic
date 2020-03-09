using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; } = null;

    public delegate void ChangeState();
    public event ChangeState changeGameModeEvent;

    
    public GameMode CurrentGameMode;
    public enum GameMode
    {

        Dialog,
        PlayerTurn,
        PlayerWeaponWait,
        EnemyTurn,
        EnemyWeaponWait,
        PauseGame,
        NextRoom, 
        Loose

    }

    public WeaponHero CurrentWeaponHero;
    public enum WeaponHero
    {

        Sword,
        Lighting,
        WaterBall,
        FireBall,
        FrozenBall        

    }

    public WeaponEnemy ChoiceWeaponEnemy;
    public enum WeaponEnemy
    {

        Sword,
        Lighting,
        WaterBall,
        FireBall,
        FrozenBall        

    }

    public HeroPawn m_HeroPawn;
    public Transform m_HeroTransform;

    private Transform nextMovePointHero;

    public int openWeapon;

    public List<Pawn> EnemyPawn = new List<Pawn>();
    public List<Transform> EnemyPawnTransform = new List<Transform>();

    [SerializeField]
    private List<Transform> camPathPoint;

    [HideInInspector]
    public Camera m_Camera;
    private Transform camTransform;
    private int camPointNumber = 0;
    
    private float currentShootStrange;
    private float timeShootLoad = 5.0f;
    private float currentTimeShootLoad = 0.0f;

    [SerializeField]
    private GameObject weaponLightingPrefab;
    private GameObject weaponLightingGO;
    private Rigidbody2D weaponLightingRB;
    private float weaponLightingSpeedMin = 20.0f;
    private float weaponLightingSpeedMax = 50.0f;
    private bool isCountTime = false;

    public GameObject hitPrefab;
    public GameObject hitRockDestroyPrefab;
    public GameObject hitSimpleLightingPrefab;

    public float Timer;

    private string sceneName;

    private int roomCount = 0;


    //для преобразования при повороте
    private Vector3 xDirection;
    private Vector3 yDirection;
    private Vector3 zDirection;

    private float tempFloat;
    private int tempInt;
    private string tempString;
    private GameObject tempGO;
    private Transform tempTr;


    private void Awake()
    {

        SearchDestroyCopySingletonOrThisCreateInstance();

        m_HeroPawn = FindObjectOfType<HeroPawn>();
        m_HeroTransform = m_HeroPawn.gameObject.GetComponent<Transform>();        

        m_Camera = Camera.main;
        camTransform = m_Camera.gameObject.transform;

        sceneName = SceneManager.GetActiveScene().name;

        GetWeaponOffValue();
        CurrentWeaponHero = WeaponHero.Sword;
        
    }

    private void Start()
    {

        ChangeGameMode(GameMode.Dialog);

    }

    private void Update()
    {

        KeyBoardHack();

        if (isCountTime && currentTimeShootLoad < timeShootLoad) currentTimeShootLoad += Time.deltaTime;

    }

    public void ChangeGameMode(GameMode m_GameMode)
    {

        CurrentGameMode = m_GameMode;

        changeGameModeEvent();

        switch (m_GameMode)
        {

            case GameMode.Dialog:
                StartCoroutine(DialogeGameMode());
                break;

            case GameMode.PlayerTurn:
                GUIManager.Instance.ShowAndHideWeaponChoice(true);
                break;

            case GameMode.PlayerWeaponWait:

                break;

            case GameMode.EnemyTurn:
                break;

        }

    }

    public void ChangeWeaponHero(WeaponHero m_WeaponHero)
    {

        CurrentWeaponHero = m_WeaponHero;

        GUIManager.Instance.ShowAndHideWeaponChoice(false);

        if (CurrentGameMode == GameMode.PlayerTurn)
        {

            switch (CurrentWeaponHero)
            {

                case WeaponHero.Sword:
                    StartCoroutine(WeaponSword());
                    break;

                case WeaponHero.Lighting:
                    StartCoroutine(WeaponLighting());
                    break;

                default:
                    StartCoroutine(WeaponSword());
                    break;

            }

        }

    }

    IEnumerator DialogeGameMode()
    {

        nextMovePointHero = LevelHeroPathPoint.GetNextPoint();

        m_HeroPawn.HeroMove(true);

        m_HeroTransform.DOMoveX(nextMovePointHero.position.x, 5.0f);

        while (Mathf.Abs(m_HeroTransform.position.x - nextMovePointHero.position.x) > 0.1f) yield return null;

        m_HeroPawn.HeroMove(false);

        GUIManager.Instance.ShowAndHideDialogWindow(true, 0);        

        while (!Input.anyKey) yield return null;

        GUIManager.Instance.ShowAndHideDialogWindow(false, 0);
        GUIManager.Instance.ShowAndHideWeaponChoice(true);

        yield return new WaitForSeconds(1.0f);

        GUIManager.Instance.OnWeaponOff();

        yield return new WaitForSeconds(1.0f);

        ChangeGameMode(GameMode.PlayerTurn);

    }
    
    IEnumerator WeaponSword()
    {

        yield return new WaitForSeconds(1.0f);

        if (!Input.GetMouseButtonUp(0))
        {

            yield return null;

        }

        m_HeroPawn.AttackSwordHero();

        yield return new WaitForSeconds(3.0f);

        ChangeGameMode(GameMode.EnemyTurn);

        yield break;

    } 
    
    IEnumerator WeaponLighting()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;

        m_HeroPawn.AttackMagicHero(true);

        weaponLightingGO = Instantiate(weaponLightingPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponLightingRB = weaponLightingGO.GetComponent<Rigidbody2D>();

        GUIManager.Instance.ShowAndHidePowerArrow(true);

        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange =(((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));
                        
            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        weaponLightingRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * (currentShootStrange + weaponLightingSpeedMin) * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }
 

    public void SetWeaponOffValue()
    {

        openWeapon++;
        //PlayerPrefs.SetInt("openWeapon", openWeapon);

    }

    public void GetWeaponOffValue()
    {

        //openWeapon = PlayerPrefs.HasKey("openWeapon") ? PlayerPrefs.GetInt("openWeapon") : 0;
        openWeapon = 0;

    }

    private void KeyBoardHack()
    {

        if (Input.GetKeyUp(KeyCode.B)) return;
        else if (Input.GetKeyUp(KeyCode.C)) return;
        else if (Input.GetKeyUp(KeyCode.W)) return;
        else if (Input.GetKeyUp(KeyCode.A)) return;

    }

    public void LoadNextScene()
    {

        ChangeGameMode(GameMode.Dialog);
        /*
        switch (sceneName)
        {

            case "001":
                SceneManager.LoadScene("002");
                break;

            case "002":
                SceneManager.LoadScene("003");
                break;

            case "003":
                SceneManager.LoadScene("001");
                break;


        }
        */
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

    private Vector3 GetOurMouseMosition()
    {

        return Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    public void LookAt2D(Vector3 lookTarget, Transform m_Transform)
    {

        //Желаемое направление оси X, от которого устанавливаем ось Y. Z ось обращена к нам.
        xDirection = (lookTarget - transform.position).normalized;
        yDirection = Quaternion.Euler(0, 0, 90) * xDirection;
        zDirection = Vector3.forward;

        m_Transform.rotation = Quaternion.LookRotation(zDirection, yDirection);

    }

}
