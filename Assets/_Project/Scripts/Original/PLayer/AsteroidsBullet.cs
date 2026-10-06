using UnityEngine;

public class AsteroidsBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;

    [Header("Camera")]
    [SerializeField] private Camera asteroidsCamera;

    private void Start()
    {
        if (asteroidsCamera == null)
        {
            GameObject cameraObject =
                GameObject.Find("AsteroidsCamera");

            if (cameraObject != null)
            {
                asteroidsCamera =
                    cameraObject.GetComponent<Camera>();
            }
        }

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Move();
        WrapAround();
    }

    private void Move()
    {
        transform.position +=
            transform.up *
            speed *
            Time.deltaTime;
    }

    private void WrapAround()
    {
        if (asteroidsCamera == null)
            return;

        Vector3 position =
            transform.position;

        Vector3 viewportPosition =
            asteroidsCamera.WorldToViewportPoint(
                position
            );

        bool changed = false;

        if (viewportPosition.x > 1f)
        {
            viewportPosition.x = 0f;
            changed = true;
        }
        else if (viewportPosition.x < 0f)
        {
            viewportPosition.x = 1f;
            changed = true;
        }

        if (viewportPosition.y > 1f)
        {
            viewportPosition.y = 0f;
            changed = true;
        }
        else if (viewportPosition.y < 0f)
        {
            viewportPosition.y = 1f;
            changed = true;
        }

        if (changed)
        {
            position =
                asteroidsCamera.ViewportToWorldPoint(
                    viewportPosition
                );

            position.z =
                transform.position.z;

            transform.position =
                position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroide"))
        {
            asteroide_original asteroid =
                other.GetComponent<asteroide_original>();

            if (asteroid != null)
            {
                asteroid.Split();
            }

            Destroy(gameObject);
        }
    }
}