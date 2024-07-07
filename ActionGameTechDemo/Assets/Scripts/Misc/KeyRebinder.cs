using UnityEngine;
using UnityEngine.UI;

public class KeyRebinder : MonoBehaviour
{
    public void OnEnable()
    {
        ShowCurrentBindings();
    }

    public void ShowCurrentBindings()
    {
        foreach (var keyBind in GetComponentInChildren<Button>())
        {

        }
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
