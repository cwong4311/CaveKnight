using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameMessageReceiver : MonoBehaviour
{
    public static Action<string, float, bool> OnMessage;

    [SerializeField]
    private TextMeshProUGUI _messageText;
    [SerializeField]
    private GameObject _messageBox;

    [SerializeField]
    private PlayerInputActionBind _actionBind;

    private string[] _remapKeywords = new string[] { "[Move]", "[Camera]", "[Roll]", "[Attack]", "[Block]", "[Lockon]", "[Swap]", "[Heal]", "[Pause]"};

    private Coroutine _currentVisibleMessage;

    public virtual void OnEnable()
    {
        _messageText.text = string.Empty;
        _messageBox.SetActive(false);

        OnMessage += OnMessageReceived;
    }

    public virtual void OnDisable()
    {
        OnMessage -= OnMessageReceived;
    }

    private void OnMessageReceived(string message, float duration, bool isGamepad)
    {
        var parsedMessage = ParseMessage(message, isGamepad);
        
        if (_currentVisibleMessage != null)
        {
            StopCoroutine(_currentVisibleMessage);
        }
        _currentVisibleMessage = StartCoroutine(ShowMessage(parsedMessage, duration));
    }

    private string ParseMessage(string rawMessage, bool isGamepad)
    {
        var parsedMessage = rawMessage;

        foreach (var keyword in _remapKeywords)
        {
            if (parsedMessage.Contains(keyword))
            {
                var remappedKeybind = "[" + GetKeybindFromKeyword(keyword, isGamepad) + "]";
                parsedMessage = parsedMessage.Replace(keyword, remappedKeybind);
            }
        }

        return parsedMessage;
    }

    private IEnumerator ShowMessage(string message, float duration)
    {
        _messageText.text = message;
        _messageBox.SetActive(true);

        yield return new WaitForSeconds(duration);

        _messageText.text = string.Empty;
        _messageBox.SetActive(false);

        _currentVisibleMessage = null;
    }

    private string GetKeybindFromKeyword(string keyword, bool isGamepad)
    {
        var deviceMask = InputBinding.MaskByGroup(isGamepad ? "Gamepad" : "KBM");

        switch (keyword)
        {
            case "[Move]":
                return isGamepad ? _actionBind.Movement.action.GetBindingDisplayString(deviceMask) : 
                    _actionBind.Movement.action.GetBindingDisplayString(0);
            case "[Camera]":
                return isGamepad ? _actionBind.Camera.action.GetBindingDisplayString(deviceMask) :
                    "Mouse";
            case "[Roll]":
                return _actionBind.Roll.action.GetBindingDisplayString(deviceMask);
            case "[Attack]":
                return _actionBind.Attack.action.GetBindingDisplayString(deviceMask);
            case "[Block]":
                return _actionBind.Block.action.GetBindingDisplayString(deviceMask);
            case "[Lockon]":
                return _actionBind.Lockon.action.GetBindingDisplayString(deviceMask);
            case "[Swap]":
                return _actionBind.SwapTarget.action.GetBindingDisplayString(deviceMask);
            case "[Heal]":
                return _actionBind.Heal.action.GetBindingDisplayString(deviceMask);
            case "[Pause]":
                return _actionBind.Pause.action.GetBindingDisplayString(deviceMask);
            default:
                return null;
        }
    }
}
