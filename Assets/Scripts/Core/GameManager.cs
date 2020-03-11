using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; } = null;

    public delegate void ChangeState();
    public event ChangeState changeGameModeEvent;

    
    public GameMode CurrentGameMode;
    public enum GameMode
    {

        MainMenu,
        Dialog,
        PlayerTurn,
        PlayerWeaponWait,
        EnemyTurn,
        EnemyWeaponWait,
        PauseGame,
        Winner,
        Loose

    }

    public WeaponHero CurrentWeaponHero;
    public enum WeaponHero
    {

        Sword,
        Lighting,
        WaterBall,
        FireBall,
        IceBall        

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

    [SerializeField]
    public List<Transform> pathPointHero;
    public int nextMovePointHero;

    public int openWeapon;
    
    [SerializeField]
    public List<Transform> camPathPoint;

    [HideInInspector]
    public Camera m_Camera;
    private Transform camTransform;
    [HideInInspector]
    public int camPointNumber = 0;
    
    private float currentShootStrange;
    private float timeShootLoad = 5.0f;
    private float currentTimeShootLoad = 0.0f;

    [SerializeField]
    private GameObject weaponLightingPrefab;    
    private float weaponLightingSpeedMin = 10.0f;
    private float weaponLightingSpeedMax = 60.0f;
    private bool isCountTime = false;

    [SerializeField]
    private GameObject weaponWaterBallPrefab;

    [SerializeField]
    private GameObject weaponFireBallPrefab;

    [SerializeField]
    private GameObject weaponIceBallPrefab;

    private GameObject weaponGO;
    private Rigidbody2D weaponRB;

    public GameObject hitPrefab;
    public GameObject hitRockDestroyPrefab;
    public GameObject hitSimpleLightingPrefab;
    public GameObject hitWaterBallExplousenPrefab;
    
    private string sceneName;

    public float levelOfComplexity = 1.0f;

    public List<EnemyRoom> enemyInRoom;
    public int enemyLiveInScene;

    [SerializeField]
    private List<GameObject> noWayBack;
    private int noWayBackCount;

    public float Timer;

    public GameObject destr;

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

        camPointNumber = -1;
        nextMovePointHero = -1;
        noWayBackCount = 0;

        SearchDestroyCopySingletonOrThisCreateInstance();

        m_HeroPawn = FindObjectOfType<HeroPawn>();
        m_HeroTransform = m_HeroPawn.gameObject.GetComponent<Transform>();
        m_HeroPawn.fakeSwordCollider.enabled = false;

        m_Camera = Camera.main;
        camTransform = m_Camera.gameObject.transform;

        sceneName = SceneManager.GetActiveScene().name;

        GetWeaponOffValue();
        CurrentWeaponHero = WeaponHero.Sword;

        foreach (EnemyRoom er in enemyInRoom) foreach (GameObject go in er.enemyGO) go.SetActive(false);
        
    }

    private void Start()
    {

        ChangeGameMode(GameMode.MainMenu);

    }

    private void Update()
    {

        KeyBoardHack();

        if (isCountTime && currentTimeShootLoad < timeShootLoad) currentTimeShootLoad += Time.deltaTime;

        if ((CurrentGameMode == GameMode.Winner | CurrentGameMode == GameMode.Loose) & Input.anyKeyDown) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

    }

    public void ChangeGameMode(GameMode m_GameMode)
    {

        if (m_GameMode != GameMode.Dialog && m_GameMode != GameMode.MainMenu && enemyLiveInScene == 0) ChangeGameMode(GameMode.Dialog);

        CurrentGameMode = m_GameMode;

        changeGameModeEvent();

        switch (m_GameMode)
        {

            case GameMode.Dialog:
                destr.SetActive(false);
                StartCoroutine(DialogeGameMode());
                break;

            case GameMode.PlayerTurn:
                GUIManager.Instance.ShowAndHideWeaponChoice(true);
                break;

            case GameMode.PlayerWeaponWait:

                break;

            case GameMode.MainMenu:
                StartCoroutine(MainMenu());
                break;

            case GameMode.EnemyTurn:
                StartCoroutine(EnemyTurn());
                break;

        }

    }

    IEnumerator MainMenu()
    {

        while (CurrentGameMode == GameMode.MainMenu)
        {



            yield return null;

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

                case WeaponHero.WaterBall:
                    StartCoroutine(WeaponWaterBall());
                    break;

                case WeaponHero.FireBall:
                    StartCoroutine(WeaponFireBall());
                    break;

                case WeaponHero.IceBall:
                    StartCoroutine(WeaponIceBall());
                    break;

                default:
                    StartCoroutine(WeaponSword());
                    break;

            }

        }

    }

    IEnumerator DialogeGameMode()
    {

        GUIManager.Instance.ShowAndHideWeaponChoice(false);        

        CameraMoveOnNextCamPathPoint();
        SetOnOffNoWayBack(false);        

        nextMovePointHero++;
        m_HeroPawn.HeroMove(true);
        m_HeroTransform.DOMoveX(pathPointHero[nextMovePointHero].position.x, Vector2.Distance(m_HeroTransform.position, pathPointHero[nextMovePointHero].position) / 2);

        while (Mathf.Abs(m_HeroTransform.position.x - pathPointHero[nextMovePointHero].position.x) > 0.1f) yield return null;

        m_HeroPawn.HeroMove(false);
        SetOnOffNoWayBack(true);

        enemyLiveInScene = enemyInRoom[camPointNumber].enemyGO.Count;
        foreach (GameObject go in enemyInRoom[camPointNumber].enemyGO) go.SetActive(true);

        GUIManager.Instance.ShowAndHideDialogWindow(true, camPointNumber);        

        while (!Input.anyKey) yield return null;

        GUIManager.Instance.ShowAndHideDialogWindow(false, camPointNumber);
        GUIManager.Instance.ShowAndHideWeaponChoice(true);

        if (camPointNumber != 1)
        {

            yield return new WaitForSeconds(1.0f);

            GUIManager.Instance.OnWeaponOff();

        }

        ChangeGameMode(GameMode.PlayerTurn);

        yield break;

    }
        
    IEnumerator WeaponSword()
    {
        
        if (!Input.GetMouseButtonUp(0))
        {

            yield return null;

        }

        m_HeroPawn.fakeSwordCollider.enabled = true;
        m_HeroPawn.AttackSwordHero();
        
        yield return new WaitForSeconds(2.0f);

        m_HeroPawn.fakeSwordCollider.enabled = false;

        ChangeGameMode(GameMode.EnemyTurn);

        yield break;

    } 
    
    IEnumerator WeaponLighting()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponGO = Instantiate(weaponLightingPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponRB = weaponGO.GetComponent<Rigidbody2D>();             
               
        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange =(((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));
                        
            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        weaponRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * 1.5f * (currentShootStrange + weaponLightingSpeedMin) * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator WeaponWaterBall()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponGO = Instantiate(weaponWaterBallPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponRB = weaponGO.GetComponent<Rigidbody2D>();
        weaponRB.bodyType = RigidbodyType2D.Kinematic;

        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange = (((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));

            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        weaponRB.bodyType = RigidbodyType2D.Dynamic;
        weaponRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * (currentShootStrange + weaponLightingSpeedMin) * 10 * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator WeaponFireBall()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponGO = Instantiate(weaponFireBallPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponRB = weaponGO.GetComponent<Rigidbody2D>();
        weaponRB.bodyType = RigidbodyType2D.Kinematic;

        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange = (((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));

            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        weaponRB.bodyType = RigidbodyType2D.Dynamic;
        weaponRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * (currentShootStrange + weaponLightingSpeedMin) * 10 * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator WeaponIceBall()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponGO = Instantiate(weaponIceBallPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponRB = weaponGO.GetComponent<Rigidbody2D>();
        weaponRB.bodyType = RigidbodyType2D.Kinematic;

        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange = (((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));

            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        weaponRB.bodyType = RigidbodyType2D.Dynamic;
        weaponRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * (currentShootStrange + weaponLightingSpeedMin) * 10 * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator EnemyTurn()
    {
        /*
        yield return new WaitForSeconds(3.0f);

        ChangeGameMode(GameMode.PlayerTurn);
        */
        if (enemyLiveInScene > 0)
        {
            
            for (int i = 0; i < enemyInRoom[camPointNumber].enemyGO.Count; i++)
            {

                GUIManager.Instance.ShowTextCurrentTurn("ход врага ", 0);

                yield return new WaitForSeconds(1.0f);

                if (enemyInRoom[camPointNumber].enemyGO[i] != null) enemyInRoom[camPointNumber].enemyGO[i].GetComponentInChildren<EnemyPawn>().Attack();

                yield return new WaitForSeconds(2.0f);

            }

            ChangeGameMode(GameMode.PlayerTurn);

        }         

    }

    public void CameraMoveOnNextCamPathPoint()
    {

        camPointNumber++;

        if (camPointNumber > 0) camTransform.DOMoveX(camPathPoint[camPointNumber].position.x, (camPathPoint[camPointNumber].position.x - camPathPoint[camPointNumber - 1].position.x) / 2);

    }

    private void SetOnOffNoWayBack(bool isFinish)
    {

        for (int i = 0; i < noWayBack.Count; i++)
        {

            if (i == camPointNumber && !isFinish) noWayBack[i].SetActive(false);
            else noWayBack[i].SetActive(true);

        }

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

        //ChangeGameMode(GameMode.Dialog);
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

    public Vector3 GetOurMouseMosition() { return Camera.main.ScreenToWorldPoint(Input.mousePosition); }

    public Transform CameraTransform() { return camTransform; }

    public void LookAt2D(Vector3 lookTarget, Transform m_Transform)
    {

        //Желаемое направление оси X, от которого устанавливаем ось Y. Z ось обращена к нам.
        xDirection = (lookTarget - transform.position).normalized;
        yDirection = Quaternion.Euler(0, 0, 90) * xDirection;
        zDirection = Vector3.forward;

        m_Transform.rotation = Quaternion.LookRotation(zDirection, yDirection);

    }

    [Serializable]
    public class EnemyRoom
    {

        public int numberRoom;
        public List<GameObject> enemyGO;

        private int tempCountGO;

        public int LiveEnemyCount()
        {

            tempCountGO = 0;

            foreach (GameObject go in enemyGO) if (go != null) tempCountGO++;

            return tempCountGO;

        }

    }

}
