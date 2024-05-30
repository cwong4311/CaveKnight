using UnityEngine;


public class FollowTransform : MonoBehaviour
{
    public Transform FollowTarget;

    public void Update()
    {
        this.transform.position = FollowTarget.position;
    }
}