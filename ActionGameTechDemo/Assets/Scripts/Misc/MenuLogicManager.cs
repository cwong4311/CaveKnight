using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogicManager : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject KeybindMenu;
    public GameObject CreditsMenu;

    public void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
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
    }

    public void ShowKeybinds()
    {
        KeybindMenu?.SetActive(true);

        HideCredits();
    }

    public void HideKeybinds()
    {
        KeybindMenu?.SetActive(false);
    }

    public void ShowCredits()
    {
        CreditsMenu?.SetActive(true);

        HideKeybinds();
    }

    public void HideCredits()
    {
        CreditsMenu?.SetActive(false);
    }

}
