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

    /// <summary>
    /// Павн и трансформ игрока. Находится в Awake.
    /// </summary>
    [HideInInspector]
    public HeroPawn m_HeroPawn;
    [HideInInspector]
    public Transform m_HeroTransform;

    /// <summary>
    /// Массив точек начала каждой новой комнаты. По ним рыцарь сам идёт после уничтожения всех противников в комнате, в которой сейчас находится.
    /// Массив заполняется сам при старте из скрипта точек LevelHeroPathPoint,cs
    /// </summary>
    [HideInInspector]
    public List<Transform> pathPointHero;
    //Следующая точка (следующая комната в которую герой автоматически перемещается)
    [HideInInspector]
    public int nextMovePointHero = -1;

    //Количество открытого игроку оружия.
    [HideInInspector]
    public int openWeapon;

    /// <summary>
    /// Массив точек камеры для каждой комнаты.
    /// Точки камеры и пути героя нужны для того, что центры комнат находятся на разном расстоянии друг от друга. 
    /// Массив заполняется сам при старте игры из скрипта CameraPathPoint, который висит на каждой точке камеры.
    /// </summary>
    [HideInInspector]
    public List<Transform> camPathPoint;

    //Самера, её трансформ и точка в которой она должна бфть сейчас.
    [HideInInspector]
    public Camera m_Camera;
    private Transform camTransform;
    [HideInInspector]
    public int camPointNumber = -1;
    
    private float currentShootStrange;
    private float timeShootLoad = 3.0f;
    private float currentTimeShootLoad = 0.0f;

    //Префабы оружия и магии. А так же их зарактеристики.
    private GameObject weaponLightingPrefab;
    private GameObject weaponLightingGO;
    private Transform weaponLightingTransform;
    private Rigidbody2D weaponLightingRB;
    private float weaponLightingSpeedMin = 10.0f;
    private float weaponLightingSpeedMax = 60.0f;
    private bool isCountTime = false;

    private GameObject weaponWaterBallPrefab;
    private GameObject weaponFireBallPrefab;
    private GameObject weaponIceBallPrefab;

    private GameObject weaponGO;
    private Rigidbody2D weaponRB;  
    
    public float levelOfComplexity = 1.0f;

    public List<EnemyRoom> enemyInRoom;

    [SerializeField]
    private List<GameObject> noWayBack;

    public float Timer;
    
    //для преобразования при повороте
    private Vector3 xDirection;
    private Vector3 yDirection;
    private Vector3 zDirection;



    private void Awake()
    {

        camPointNumber = -1;
        nextMovePointHero = -1;

        SearchDestroyCopySingletonOrThisCreateInstance();

        m_HeroPawn = FindObjectOfType<HeroPawn>();
        m_HeroTransform = m_HeroPawn.gameObject.GetComponent<Transform>();
        m_HeroPawn.fakeSwordCollider.enabled = false;

        m_Camera = Camera.main;
        camTransform = m_Camera.gameObject.transform;

        GetWeaponOffValue();
        CurrentWeaponHero = WeaponHero.Sword;

        foreach (EnemyRoom er in enemyInRoom)
        {
            
            foreach (GameObject go in er.enemyGO)
            {

                er.enemyPW.Add(go.GetComponentInChildren<EnemyPawn>());
                er.enemyPWGO.Add(er.enemyPW[er.enemyPW.Count - 1].gameObject);
                go.SetActive(false);

            }

        }

        levelOfComplexity = PlayerPrefs.HasKey("LevelOfComplexity") ? PlayerPrefs.GetFloat("LevelOfComplexity") : 0.75f;

        weaponLightingPrefab = Resources.Load("WeaponMagicLightingCircle") as GameObject;
        weaponLightingGO = Instantiate(weaponLightingPrefab);
        weaponLightingTransform = weaponLightingGO.transform;
        weaponLightingRB = weaponLightingGO.GetComponent<Rigidbody2D>();
        weaponLightingGO.SetActive(false);

        weaponWaterBallPrefab = Resources.Load("WeaponMagicWaterBall") as GameObject;
        weaponFireBallPrefab = Resources.Load("WeaponMagicFireBall") as GameObject;
        weaponIceBallPrefab = Resources.Load("WeaponMagicIceBall") as GameObject;

    }

    private void Start()
    {

        ChangeGameMode(GameMode.Dialog);

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

        if (m_GameMode != GameMode.Dialog && enemyInRoom[camPointNumber].enemyGO.Count == 0) ChangeGameMode(GameMode.Dialog);

        CurrentGameMode = m_GameMode;

        changeGameModeEvent();

        switch (m_GameMode)
        {

            case GameMode.Dialog:
                StartCoroutine(DialogeGameMode());
                break;

            case GameMode.PlayerTurn:
                GUIManager.Instance.ShowAndHideWeaponChoice(true);
                m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.Move);
                break;

            case GameMode.PlayerWeaponWait:

                break;

            case GameMode.EnemyTurn:
                StartCoroutine(EnemyTurn());
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
        m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.MoveAutomate);
        m_HeroTransform.DOMoveX(pathPointHero[nextMovePointHero].position.x, Vector2.Distance(m_HeroTransform.position, pathPointHero[nextMovePointHero].position) / 2);

        while (Mathf.Abs(m_HeroTransform.position.x - pathPointHero[nextMovePointHero].position.x) > 0.1f) yield return null;

        m_HeroPawn.HeroMove(false);
        SetOnOffNoWayBack(true);

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

        m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.Move);
        ChangeGameMode(GameMode.PlayerTurn);        

        yield break;

    }
        
    IEnumerator WeaponSword()
    {
        
        if (!Input.GetMouseButtonUp(0))
        {

            yield return null;

        }

        m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.Attack);

        m_HeroPawn.fakeSwordCollider.enabled = true;
        m_HeroPawn.AttackSwordHero();

        yield return new WaitForSeconds(0.5f);

        SoundAndMusic.Instance.PlayWeaponAttack();

        yield return new WaitForSeconds(1.5f);

        m_HeroPawn.fakeSwordCollider.enabled = false;

        ChangeGameMode(GameMode.EnemyTurn);

        yield break;

    } 
    
    IEnumerator WeaponLighting()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.Attack);
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponLightingGO.SetActive(true);
        weaponLightingTransform.position = m_HeroPawn.shootPoint.position;

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(true);
               
        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange =(((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));
                        
            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(false);

        weaponLightingRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * 1.5f * (currentShootStrange + weaponLightingSpeedMin) * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator WeaponWaterBall()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.Attack);
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponGO = Instantiate(weaponWaterBallPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponRB = weaponGO.GetComponent<Rigidbody2D>();
        weaponRB.bodyType = RigidbodyType2D.Kinematic;

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(true);

        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange = (((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));

            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(false);

        weaponRB.bodyType = RigidbodyType2D.Dynamic;
        weaponRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * (currentShootStrange + weaponLightingSpeedMin) * 10 * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator WeaponFireBall()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.Attack);
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponGO = Instantiate(weaponFireBallPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponRB = weaponGO.GetComponent<Rigidbody2D>();
        weaponRB.bodyType = RigidbodyType2D.Kinematic;

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(true);

        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange = (((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));

            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(false);

        weaponRB.bodyType = RigidbodyType2D.Dynamic;
        weaponRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * (currentShootStrange + weaponLightingSpeedMin) * 10 * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator WeaponIceBall()
    {

        while (!Input.GetMouseButtonDown(0)) yield return null;
        m_HeroPawn.ChangeHeroState(HeroPawn.HeroState.Attack);
        m_HeroPawn.AttackMagicHero(true);
        GUIManager.Instance.ShowAndHidePowerArrow(true);
        currentTimeShootLoad = 0.0f;
        isCountTime = true;

        weaponGO = Instantiate(weaponIceBallPrefab, m_HeroPawn.shootPoint.position, Quaternion.identity);
        weaponRB = weaponGO.GetComponent<Rigidbody2D>();
        weaponRB.bodyType = RigidbodyType2D.Kinematic;

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(true);

        while (!Input.GetMouseButtonUp(0))
        {

            currentShootStrange = (((weaponLightingSpeedMax - weaponLightingSpeedMin) * currentTimeShootLoad) / timeShootLoad);
            GUIManager.Instance.SetPowerArrowSliderValue(currentShootStrange / (weaponLightingSpeedMax - weaponLightingSpeedMin));

            yield return null;

        }

        isCountTime = false;
        GUIManager.Instance.ShowAndHidePowerArrow(false);
        m_HeroPawn.AttackMagicHero(false);

        SoundAndMusic.Instance.PlayAndStopPowerUpSound(false);

        weaponRB.bodyType = RigidbodyType2D.Dynamic;
        weaponRB.AddForce((GetOurMouseMosition() - m_HeroPawn.shootPoint.position).normalized * (currentShootStrange + weaponLightingSpeedMin) * 10 * Time.deltaTime, ForceMode2D.Impulse);

        ChangeGameMode(GameMode.PlayerWeaponWait);

        yield break;

    }

    IEnumerator EnemyTurn()
    {

        if (enemyInRoom[camPointNumber].enemyGO.Count > 0)
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

        if (camPointNumber >= 0) SoundAndMusic.Instance.PlayRoomClearMusic();               

        camPointNumber++;

        if (camPointNumber >= 6 & camPointNumber <= 8) SoundAndMusic.Instance.ChekAndPlayWindSound(true);
        else SoundAndMusic.Instance.ChekAndPlayWindSound(false);

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

    }

    public void GetWeaponOffValue()
    {
        //0
        openWeapon = 0;

    }
    
    private void KeyBoardHack()
    {

        if (Input.GetKeyUp(KeyCode.B)) return;
        else if (Input.GetKeyUp(KeyCode.C)) return;
        else if (Input.GetKeyUp(KeyCode.W)) return;
        else if (Input.GetKeyUp(KeyCode.A)) return;

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
        public List<EnemyPawn> enemyPW;
        public List<GameObject> enemyPWGO;      

    }

}
