using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

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

    public List<Pawn> EnemyPawn = new List<Pawn>();
    public List<Transform> EnemyPawnTransform = new List<Transform>();

    [SerializeField]
    private List<GameObject> StartRoomPointGO = new List<GameObject>();
    private List<Transform> StartRoomPointTR = new List<Transform>();

    [SerializeField]
    private List<GameObject> EndRoomPointGO = new List<GameObject>();
    private List<Transform> EndRoomPointTR = new List<Transform>();

    [HideInInspector]
    public Camera m_Camera;
    private Transform camTransform;

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

        CurrentGameMode = GameMode.Dialog;

        m_Camera = Camera.main;
        camTransform = m_Camera.gameObject.transform;

        sceneName = SceneManager.GetActiveScene().name;

    }

    private void Update()
    {

        KeyBoardHack();

    }

    public void ChangeGameMode(GameMode m_GameMode)
    {

        CurrentGameMode = m_GameMode;

        EventChangeGameMode();

        switch (m_GameMode)
        {

            case GameMode.Dialog:
                
                break;

            case GameMode.PlayerTurn:

                break;

            case GameMode.PlayerWeaponWait:
                
                break;

        }

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
