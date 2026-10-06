using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    [SerializeField] public float speed = 1f;
    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            speed * Time.deltaTime,
            Space.Self
        );
    }
}
