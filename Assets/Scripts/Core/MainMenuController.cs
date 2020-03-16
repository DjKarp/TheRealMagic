using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление меню.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    
    private Animator animatorMainMenu;
    private float timer;


    public MainMenuState currentMainMenuState;
    public enum MainMenuState
    {

        RedLegGamesLogo,
        GameHistory,
        MainMenu,
        Loading

    }

    private void Awake()
    {

        animatorMainMenu = gameObject.GetComponent<Animator>();
        timer = 0.0f;

    }

    private void Update()
    {

        if (currentMainMenuState == MainMenuState.RedLegGamesLogo) ShowRedLegGamesLogo();
        else if (currentMainMenuState == MainMenuState.GameHistory) ShowGameHistory();

    }

    public void ChangeMainMenuState(MainMenuState m_MainMenuState)
    {

        currentMainMenuState = m_MainMenuState;

        switch (currentMainMenuState)
        {

            case MainMenuState.GameHistory:
                animatorMainMenu.SetTrigger("isRedLegGamesLogoOff");
                timer = 0.0f;
                break;

            case MainMenuState.MainMenu:
                animatorMainMenu.SetTrigger("isGameHistoryOff");
                timer = 0.0f;
                break;

            case MainMenuState.Loading:
                SceneManager.LoadSceneAsync(1);
                break;

        }

    }

    private void ShowRedLegGamesLogo()
    {

        timer += Time.deltaTime;
        if ((timer < 7 & Input.anyKeyDown) | timer > 7) ChangeMainMenuState(MainMenuState.GameHistory);

    }

    private void ShowGameHistory()
    {

        timer += Time.deltaTime;
        if ((timer < 15 & Input.anyKeyDown) | timer > 15) ChangeMainMenuState(MainMenuState.MainMenu);

    }

    public void SetDiffyclyEnd()
    {

        ChangeMainMenuState(MainMenuState.Loading);

    }

}
