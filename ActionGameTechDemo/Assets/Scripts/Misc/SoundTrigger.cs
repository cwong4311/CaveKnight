using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public LayerMask TriggerOnLayer;

    public string SoundName;

    public void OnTriggerEnter(Collider other)
    {
        if ((TriggerOnLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            BGMMusicManager.OnMusicChange(SoundName);
        }
    }
}
