using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "PlayerInputActionBind", order = 1)]
public class PlayerInputActionBind : ScriptableObject
{
    public InputActionReference Movement;
    public InputActionReference Camera;
    public InputActionReference Roll;
    public InputActionReference Attack;
    public InputActionReference Block;
    public InputActionReference Lockon;
    public InputActionReference Heal;
    public InputActionReference Pause;
}