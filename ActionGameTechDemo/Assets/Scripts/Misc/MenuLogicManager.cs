using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogicManager : MonoBehaviour
{
    public GameObject SettingsMenu;
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

    public void ShowCredits()
    {
        CreditsMenu?.SetActive(true);
    }

    public void HideCreadits()
    {
        CreditsMenu?.SetActive(false);
    }

    public void ShowSettings()
    {
        SettingsMenu?.SetActive(true);
    }

    public void HideSettings()
    {
        SettingsMenu?.SetActive(false);
    }
}
