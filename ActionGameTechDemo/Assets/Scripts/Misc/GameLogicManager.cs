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
    public PlayerController Player;

    public AudioSource MenuAudio;
    public AudioClip GameOverSound;
    public AudioClip YouWinSound;
    public AudioMixerSnapshot DefaultMixerSnapshot;
    public AudioMixerSnapshot GameOverMixerSnapshot;
    public AudioMixerSnapshot PausedMixerSnapshot;

    public static Action OnGameOver;
    public static Action OnLevelComplete;

    private Coroutine _currentGameEndScreen;

    public void OnEnable()
    {
        _currentGameEndScreen = null;
        PlayMenuAnimation("Intro");

        OnGameOver += PlayGameOverScreen;
        OnLevelComplete += PlayLevelCompletedScreen;

        if (MenuAudio == null) MenuAudio = GetComponent<AudioSource>();
        DefaultMixerSnapshot.TransitionTo(.01f);
    }

    public void OnDisable()
    {
        OnGameOver = null;
        OnLevelComplete = null;
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

    private void ReturnToMainMenu()
    {

    }
}
