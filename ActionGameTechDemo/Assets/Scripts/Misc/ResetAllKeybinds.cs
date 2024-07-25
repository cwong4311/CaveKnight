using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllKeybinds : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset _inputAction;

    public void ResetAllBindings()
    {
        foreach (InputActionMap map in _inputAction.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
    }
}
