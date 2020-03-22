using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Синглтон, отвечающий за всё музыкальное сопровождение. Музыку и звуки задаёт автоматически в соответствии со списком уровней и привязанных
/// к ним звуков. Именно синглтон нужен, так как FMOD умеет не прерывать музыку между загрузками сцен, а одиночка как раз и следит за тем, 
/// надо ли переключать композициию.
/// 
/// Доступ из любого скрипта проекта -> SoundAndMusic.Instance. <-
/// 
/// </summary> 
public class SoundAndMusic : MonoBehaviour
{

    private static SoundAndMusic _instance = null;
    
    public VCA MusicVCA;
    public VCA FXVCA;

    private Camera m_Camera;
    private GameObject m_CameraGO;

    public GameObject fmodGO;
    private Transform fmodTransform;
    
    /// <summary>
    /// /////////////////////////// = Sound And Music path in FMOD = /////////////////////////////////////
    /// </summary>

    //Game - Variable

    /// <summary>
    /// Ниже идёт список эвентов FMOD. А так же индивидуальные эвенты.
    /// </summary>
    //Melody
    private string menuMusic = "event:/Music/MainMenuMusic";
    private string gameMusic = "event:/Music/GameMusic";
    private string winnerSound = "event:/GUI/Winner";                                       //Мелодия победы
    private string looseSound = "event:/GUI/Looser";                                        //Мелодия поражения     
    private string roomClear = "event:/GUI/RoomClear";

    //GUI  
    private string clickOnGUIinMainMenu = "event:/GUI/MenuClick";
    private string clickOnWeaponChoise = "event:/GUI/WeaponChoise";
    private string playerTurnBegin = "event:/GUI/PlayerTurn";
    private string powerUpStart = "event:/GUI/PowerUP";

    //FX
    private string takePoisen = "event:/FX/TakePoisen";
    private string wind = "event:/FX/Wind";
    private string rikoshet = "event:/FX/Rikoshet";

    //Weapon
    private string sword = "event:/Weapon/SwordAttack";
    private string startWater = "event:/Weapon/WaterStart";
    private string damageWater = "event:/Weapon/WaterDamage";
    private string startFire = "event:/Weapon/FireStart";
    private string damageFire = "event:/Weapon/FireDamage";
    private string startLighting = "event:/Weapon/LightingStart";
    private string damageLighting = "event:/Weapon/LightingDamage";
    private string startIce = "event:/Weapon/IceStart";
    private string damageIce = "event:/Weapon/IceDamage";

    //Enemy
    private string attackEnemy= "event:/Enemy/Attack_Enemy";
    private string attackGhost = "event:/Enemy/Attack_Ghost";
    private string damagedEnemy = "event:/Enemy/Damaged_Enemy";
    private string damagedGhost = "event:/Enemy/Damaged_Ghost";
    private string dieEnemy = "event:/Enemy/Die_Enemy";
    private string dieGhost = "event:/Enemy/Die_Ghost";

    //FMOD Events
    private EventInstance musicEvent;
    private EventInstance windEvent;
    private EventInstance powerUpEvent;

    private EventDescription myEvDes;
    private string tempPath;

    private int sceneBuildIndex;

    //
    [HideInInspector]
    public EnemyType m_EnemyType;
    public enum EnemyType
    {

        Enemy,
        Ghost

    }


    public static SoundAndMusic Instance
    {

        get
        {
            if (_instance == null)
            {

                _instance = FindObjectOfType<SoundAndMusic>();

                if (_instance == null)
                {

                    GameObject go = new GameObject();
                    go.name = "SingletonController";
                    _instance = go.AddComponent<SoundAndMusic>();
                    DontDestroyOnLoad(go);

                }

            }

            return _instance;

        }

    }

    /// <summary>
    /// /////////////////////////// = Initializations = /////////////////////////////////////
    /// </summary>
    void Awake()
    {

        if (_instance == null)
        {

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
                        
        }
        else
        {

            Destroy(gameObject);

        }

    }
    void Start() { InitializeManager(); }
    private void InitializeManager()
    {

        sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        CheckCamera();

        //MusicVCA = RuntimeManager.GetVCA("vca:/Music Bank");
        //FXVCA = RuntimeManager.GetVCA("vca:/FX Bank");

        CreateFMODGO();

        InitializeFMODEvents();
        ChangePlayingMusic();

        if (sceneBuildIndex > 0) GameManager.Instance.changeGameModeEvent += OnChangeGameState;
        
    }

    /// <summary>
    /// Так как в каждой сцене своя камера, то нужно следить, чтобы на неё всегда была ссылка. 
    /// Из камеры должны звучать все звуки и мелодии связанные с интерфейсом и т.п.
    /// </summary>
    public void CheckCamera()
    {

        if (m_Camera == null) m_Camera = Camera.main;
        if (m_CameraGO == null && m_Camera != null && m_Camera.gameObject != null) m_CameraGO = m_Camera.gameObject;
        if (m_CameraGO == null) m_CameraGO = gameObject;

    }

    /// <summary>
    /// fmodGO - объект из точки которого, должны проигрываться некоторые звуки.
    /// FMOD очень хорошо может воспроизводить звук, показывая игроку наглядно - откуда он звучит.
    /// </summary>
    private void CreateFMODGO()
    {

        fmodGO = new GameObject();
        fmodTransform = fmodGO.transform;
        fmodGO.name = "fmodGO";

    }
    private void InitializeFMODEvents()
    {

        windEvent = RuntimeManager.CreateInstance(wind);
        windEvent.set3DAttributes(RuntimeUtils.To3DAttributes(m_CameraGO));

        powerUpEvent = RuntimeManager.CreateInstance(powerUpStart);
        powerUpEvent.set3DAttributes(RuntimeUtils.To3DAttributes(m_CameraGO));

    }

    private void ChangePlayingMusic()
    {

        if (sceneBuildIndex == 0)
        {

            musicEvent = RuntimeManager.CreateInstance(menuMusic);
            musicEvent.start();

        }
        else
        {

            if (GetMusicPath() != null) StopMusic();
            musicEvent = RuntimeManager.CreateInstance(gameMusic);
            musicEvent.start();


        }

    }

    private void ChangePlayingMusicAwake()
    {

        if (sceneBuildIndex == 0)
        {

            musicEvent = RuntimeManager.CreateInstance(menuMusic);
            musicEvent.start();

        }
        else
        {

            if (GetMusicPath() != null) StopMusic();
            musicEvent = RuntimeManager.CreateInstance(gameMusic);
            musicEvent.start();


        }

    }
    public void ChangePlayingMusic(int i)
    {

        PlayClickOnGUIinMainMenu();

        StopMusic();
        musicEvent = RuntimeManager.CreateInstance(gameMusic);
        musicEvent.start();

        CheckCamera();

    }

    public void StopMusic()
    {

        musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }

    public string GetMusicPath()
    {

        musicEvent.getDescription(out myEvDes);
        myEvDes.getPath(out tempPath);
        return tempPath;

    }

    public void OnChangeGameState()
    {
        
        switch (GameManager.Instance.CurrentGameMode)
        {

            case GameManager.GameMode.Dialog:
                //ChangePlayingMusic(1);
                break;

            case GameManager.GameMode.PlayerTurn:
                PlayPlayerTurnSound();
                break;

            case GameManager.GameMode.Loose:
                PlayLooseMusic();
                break;

            case GameManager.GameMode.Winner:
                PlayWinMusic();
                break;

        }

    }

    void OnDestroy()
    {

        //звуки победы поражения
        //GameManager.Instance.changeGameModeEvent -= OnChangeGameState;

    }

    /// <summary>
    /// /////////////////////////// = Sound And Music Methods = /////////////////////////////////////
    /// 
    /// Общедоступные методы для вызова их через синглтон. 
    /// Здесь и надо создавать новые методы для новых звуков в игре.
    /// Заделены по зонам ответственности для более лёгкой навигаии.
    /// 
    /// </summary>

    //////////// = MENU = ///////////////
    public void PlayClickOnGUIinMainMenu()
    {

        RuntimeManager.PlayOneShotAttached(clickOnGUIinMainMenu, m_CameraGO);

    }

    ///////////// = GUI = ///////////////
    public void SetMusicVolume(float volume)
    {

        //MusicVCA.setVolume(volume);
        //GameManager._SoundAndMusicSettings.volumeMusic = volume;

    }
    public void SetFXVolume(float volume)
    {

        //FXVCA.setVolume(volume);
        //GameManager._SoundAndMusicSettings.volumeFX = volume;

    }
    public void PlayWinMusic()
    {
                                                    
        RuntimeManager.PlayOneShotAttached(winnerSound, m_CameraGO);

    }
    public void PlayLooseMusic()
    {

        RuntimeManager.PlayOneShotAttached(looseSound, m_CameraGO);

    }

    public void PlayChoiseWeaponSound()
    {

        if (m_CameraGO == null) CheckCamera();
        else RuntimeManager.PlayOneShotAttached(clickOnWeaponChoise, m_CameraGO);

    }

    public void PlayPlayerTurnSound()
    {

        if (m_CameraGO == null) CheckCamera();
        else RuntimeManager.PlayOneShotAttached(playerTurnBegin, m_CameraGO);

    }

    public void PlayAndStopPowerUpSound(bool isPowerUpStart)
    {

        if (isPowerUpStart) powerUpEvent.start();
        else
        {

            powerUpEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            PlayWeaponAttack();

        }
        
    }

    public void PlayRoomClearMusic()
    {

        CheckCamera();
        RuntimeManager.PlayOneShotAttached(roomClear, m_CameraGO);

    }

    ///////////// = GAME = ///////////////
    //////// = ENEMY = //////////
    public void PlayEnemyAttack(EnemyType m_EnemyType)
    {

        switch (m_EnemyType)
        {

            case EnemyType.Enemy:
                RuntimeManager.PlayOneShotAttached(attackEnemy, m_CameraGO);
                break;

            case EnemyType.Ghost:
                RuntimeManager.PlayOneShotAttached(attackGhost, m_CameraGO);
                break;

        }

    }
    public void PlayEnemyDamaged(EnemyType m_EnemyType)
    {

        switch (m_EnemyType)
        {

            case EnemyType.Enemy:
                RuntimeManager.PlayOneShotAttached(damagedEnemy, m_CameraGO);
                break;

            case EnemyType.Ghost:
                RuntimeManager.PlayOneShotAttached(damagedGhost, m_CameraGO);
                break;

        }

    }
    public void PlayEnemyDie(EnemyType m_EnemyType)
    {

        switch (m_EnemyType)
        {

            case EnemyType.Enemy:
                RuntimeManager.PlayOneShotAttached(dieEnemy, m_CameraGO);
                break;

            case EnemyType.Ghost:
                RuntimeManager.PlayOneShotAttached(dieGhost, m_CameraGO);
                break;

        }

    }

    //////// = WEAPON = //////////
    public void PlayWeaponAttack()
    {

        switch (GameManager.Instance.CurrentWeaponHero)
        {

            case GameManager.WeaponHero.Sword:
                RuntimeManager.PlayOneShotAttached(sword, m_CameraGO);
                break;

            case GameManager.WeaponHero.Lighting:
                RuntimeManager.PlayOneShotAttached(startLighting, m_CameraGO);
                break;

            case GameManager.WeaponHero.WaterBall:
                RuntimeManager.PlayOneShotAttached(startWater, m_CameraGO);
                break;

            case GameManager.WeaponHero.FireBall:
                RuntimeManager.PlayOneShotAttached(startFire, m_CameraGO);
                break;

            case GameManager.WeaponHero.IceBall:
                RuntimeManager.PlayOneShotAttached(startIce, m_CameraGO);
                break;

        }       

    }
    public void PlayWeaponDamage()
    {

        switch (GameManager.Instance.CurrentWeaponHero)
        {
            
            case GameManager.WeaponHero.Lighting:
                RuntimeManager.PlayOneShotAttached(damageLighting, m_CameraGO);
                break;

            case GameManager.WeaponHero.WaterBall:
                RuntimeManager.PlayOneShotAttached(damageWater, m_CameraGO);
                break;

            case GameManager.WeaponHero.FireBall:
                RuntimeManager.PlayOneShotAttached(damageFire, m_CameraGO);
                break;

            case GameManager.WeaponHero.IceBall:
                RuntimeManager.PlayOneShotAttached(damageIce, m_CameraGO);
                break;

        }

    }

    public void PlayLightingRicoshetSound()
    {

        RuntimeManager.PlayOneShotAttached(rikoshet, m_CameraGO);

    }

    //////// = OTHER = //////////
    public void PlayTakePoisenSound()
    {

        RuntimeManager.PlayOneShotAttached(takePoisen, m_CameraGO);

    }

    public void ChekAndPlayWindSound(bool isHaweWindInRoom)
    {

        if (isHaweWindInRoom) windEvent.start();
        else windEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }

}
