using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameLogicManager : MonoBehaviour
{
    public Animator Menu;
    public GameObject PauseScreen;
    public PlayerController Player;

    public AudioSource MenuAudio;
    public AudioClip GameOverSound;
    public AudioClip YouWinSound;
    public AudioMixerSnapshot DefaultMixerSnapshot;
    public AudioMixerSnapshot GameOverMixerSnapshot;
    public AudioMixerSnapshot PausedMixerSnapshot;

    public static Action OnPause;
    public static Action OnGameOver;
    public static Action OnLevelComplete;

    public static bool IsPaused;

    private Coroutine _currentGameEndScreen;
    private float _originalTimeScale;

    public void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        IsPaused = false;
        _currentGameEndScreen = null;

        PlayMenuAnimation("Intro");

        if (MenuAudio == null) MenuAudio = GetComponent<AudioSource>();
        DefaultMixerSnapshot.TransitionTo(.01f);

        OnGameOver += PlayGameOverScreen;
        OnLevelComplete += PlayLevelCompletedScreen;
        OnPause += OnPausePressed;
    }

    public void OnDisable()
    {
        OnGameOver = null;
        OnLevelComplete = null;
        OnPause = null;
    }

    protected void PlayGameOverScreen()
    {
        if (_currentGameEndScreen != null) return;

        // Play gameover sound immediately. Includes Player onDeath
        MenuAudio?.PlayOneShot(GameOverSound);
        GameOverMixerSnapshot.TransitionTo(3f);

        // Display GameEnd anim after 2 second
        StartCoroutine(
            PerformAfterDelay(2f, 
            () => {
                PlayMenuAnimation("GameOver"); 
            })
        );

        //Reload the scene after GameOver anim finishes playing
        _currentGameEndScreen = StartCoroutine(
            PerformAfterDelay(9f,
            () => ReloadLevel())
        );
    }

    protected void PlayLevelCompletedScreen()
    {
        if (_currentGameEndScreen != null) return;

        _currentGameEndScreen = StartCoroutine(
            PerformAfterDelay(3f,
            () => {
                MenuAudio?.PlayOneShot(YouWinSound);
                PlayMenuAnimation("YouWin");
            })
        );
    }

    private void PlayMenuAnimation(string animName)
    {
        Menu?.SetTrigger(animName);
    }

    private IEnumerator PerformAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected void OnPausePressed()
    {
        var isNowPaused = !IsPaused;
        if (isNowPaused)
        {
            Pause();
        }
        else
        {
            UnPause();
        }
    }

    public void Pause()
    {
        if (IsPaused) return;
        if (_currentGameEndScreen != null) return;

        IsPaused = true;

        _originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PauseScreen.SetActive(true);

        PausedMixerSnapshot.TransitionTo(.01f);
    }

    public void UnPause()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = _originalTimeScale;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PauseScreen.SetActive(false);

        DefaultMixerSnapshot.TransitionTo(0.5f);
    }

    public void ReturnToMainMenu()
    {

    }
}
