using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public LayerMask TriggerOnLayer;

    public CheckpointManager CheckpointManager;
    public int CheckpointID;

    public void OnTriggerEnter(Collider other)
    {
        if ((TriggerOnLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            this.CheckpointManager.UpdateCheckpoint(CheckpointID);
        }
    }
}
