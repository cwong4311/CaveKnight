using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMessageSender : MonoBehaviour
{
    public LayerMask TriggerOnLayer;
    [TextArea]
    public string MessageToDisplay;
    public float DisplayDuration;

    public void OnTriggerEnter(Collider other)
    {
        if ((TriggerOnLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            var playerInput = other.GetComponent<PlayerInputHandler>();
            if (playerInput == null) return;

            var isGamepad = playerInput.IsGamepad;
            GameMessageReceiver.OnMessage(MessageToDisplay, DisplayDuration, isGamepad);
            Destroy(gameObject);
        }
    }
}
