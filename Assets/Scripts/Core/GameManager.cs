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
    

    public event Action EventChangeGameMode;

    
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

    public float Timer;

    private string sceneName;

    private int roomCount = 0;



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
        
    }

    private void Start()
    {

        ChangeGameMode(GameMode.Dialog);

    }

    private void Update()
    {

        KeyBoardHack();

    }

    public void ChangeGameMode(GameMode m_GameMode)
    {

        CurrentGameMode = m_GameMode;

        //EventChangeGameMode();

        switch (m_GameMode)
        {

            case GameMode.Dialog:
                StartCoroutine(DialogeGameMode());
                break;

            case GameMode.PlayerTurn:
                StartCoroutine(PlayerTurnGameMode());
                break;

            case GameMode.PlayerWeaponWait:
                
                break;

        }

    }

    IEnumerator DialogeGameMode()
    {

        nextMovePointHero = LevelHeroPathPoint.GetNextPoint();

        m_HeroPawn.HeroMove(true);

        while (Mathf.Abs(m_HeroTransform.position.x - nextMovePointHero.position.x) > 0.1f)
        {
            
            m_HeroTransform.DOMoveX(nextMovePointHero.position.x, 5.0f);

            yield return null;

        }

        m_HeroPawn.HeroMove(false);

        GUIManager.Instance.ShowAndHideDialogWindow(true, 0);        

        while (!Input.anyKey)
        {

            yield return null;

        }

        GUIManager.Instance.ShowAndHideDialogWindow(false, 0);
        GUIManager.Instance.ShowAndHideWeaponChoice(true);

        yield return new WaitForSeconds(1.0f);

        GUIManager.Instance.OnWeaponOff();

        yield return new WaitForSeconds(1.0f);

        ChangeGameMode(GameMode.PlayerTurn);

    }

    IEnumerator PlayerTurnGameMode()
    {

        GUIManager.Instance.ShowAndHideWeaponChoice(true);

        yield return null; 

    }



    public void ChangeWeaponHero(WeaponHero m_WeaponHero)
    {

        CurrentWeaponHero = m_WeaponHero;

        GUIManager.Instance.ShowAndHideWeaponChoice(false);

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

}
