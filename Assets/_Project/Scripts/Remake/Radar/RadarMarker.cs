using UnityEngine;

public class RadarMarker : MonoBehaviour
{
    [HideInInspector]
    public Transform target;

    [HideInInspector]
    public RadarController radar;

    [HideInInspector]
    public Vector3 desiredPosition;

    [SerializeField]
    private float smoothSpeed = 8f;

    void Update()
    {
        transform.localPosition =
            Vector3.Lerp(
                transform.localPosition,
                desiredPosition,
                Time.deltaTime * smoothSpeed);
    }
}