using System.Collections;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuLogicManager : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject KeybindMenu;
    public GameObject CreditsMenu;
    public GameObject LoadingScreen;

    public GameObject MainMenuFirst;
    public GameObject KeybindMenuFirst;
    public GameObject CreditMenuFirst;

    private Coroutine _gameLoadCoroutine;

    public void OnEnable()
    {
        if (_gameLoadCoroutine != null)
        {
            StopCoroutine(_gameLoadCoroutine);
            _gameLoadCoroutine = null;
        }

        SettingsMenu.SetActive(false);
        LoadingScreen.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EventSystem.current.SetSelectedGameObject(MainMenuFirst);
    }

    public void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(MainMenuFirst);
        }
    }

    public void StartGame()
    {
        if (_gameLoadCoroutine != null)
        {
            return;
        }

        _gameLoadCoroutine = StartCoroutine(StartGameCoroutine());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSettings()
    {
        SettingsMenu?.SetActive(true);

        ShowKeybinds();
        HideCredits();
    }

    public void HideSettings()
    {
        SettingsMenu?.SetActive(false);
        EventSystem.current.SetSelectedGameObject(MainMenuFirst);
    }

    public void ShowKeybinds()
    {
        KeybindMenu?.SetActive(true);
        EventSystem.current.SetSelectedGameObject(KeybindMenuFirst);

        HideCredits();
    }

    public void HideKeybinds()
    {
        KeybindMenu?.SetActive(false);
    }

    public void ShowCredits()
    {
        CreditsMenu?.SetActive(true);
        EventSystem.current.SetSelectedGameObject(CreditMenuFirst);

        HideKeybinds();
    }

    public void HideCredits()
    {
        CreditsMenu?.SetActive(false);
    }


    private IEnumerator StartGameCoroutine()
    {
        LoadingScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
