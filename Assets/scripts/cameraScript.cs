using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z; // сохраняем Z камеры

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}