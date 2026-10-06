using UnityEngine;

public class asteroide_original : MonoBehaviour
{
    public enum AsteroidSize
    {
        Grande,
        Medio,
        Pequeno
    }

    [Header("Asteroid")]
    [SerializeField] private AsteroidSize size = AsteroidSize.Grande;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 50f;

    [Header("Camera")]
    [SerializeField] private Camera asteroidsCamera;

    [Header("Split")]
    [SerializeField] private GameObject asteroidPrefab;

    private Vector3 movementDirection;

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

        movementDirection =
            Random.insideUnitCircle.normalized;

        movementDirection =
            new Vector3(
                movementDirection.x,
                movementDirection.y,
                0f
            );
    }

    private void Update()
    {
        Move();
        Rotate();
        WrapAround();
    }

    private void Move()
    {
        transform.position +=
            movementDirection *
            speed *
            Time.deltaTime;
    }

    private void Rotate()
    {
        transform.Rotate(
            Vector3.forward *
            rotationSpeed *
            Time.deltaTime
        );
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

    public void Split()
    {
        if (size == AsteroidSize.Pequeno)
        {
            // Pequeno = 100 pontos
            if (OriginalAsteroidsScore.Instance != null)
            {
                OriginalAsteroidsScore.Instance.AddScore(100);
            }

            Destroy(gameObject);
            return;
        }

        AsteroidSize nextSize;

        if (size == AsteroidSize.Grande)
        {
            // Grande = 20 pontos
            if (OriginalAsteroidsScore.Instance != null)
            {
                OriginalAsteroidsScore.Instance.AddScore(20);
            }

            nextSize = AsteroidSize.Medio;
        }
        else
        {
            // Médio = 50 pontos
            if (OriginalAsteroidsScore.Instance != null)
            {
                OriginalAsteroidsScore.Instance.AddScore(50);
            }

            nextSize = AsteroidSize.Pequeno;
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject newAsteroid =
                Instantiate(
                    asteroidPrefab,
                    transform.position,
                    Quaternion.identity
                );

            asteroide_original asteroidScript =
                newAsteroid.GetComponent<asteroide_original>();

            if (asteroidScript != null)
            {
                asteroidScript.SetSize(nextSize);
            }
        }

        Destroy(gameObject);
    }

    public void SetSize(AsteroidSize newSize)
    {
        size = newSize;

        switch (size)
        {
            case AsteroidSize.Grande:

                transform.localScale =
                    Vector3.one * 1f;

                speed = 2f;

                break;

            case AsteroidSize.Medio:

                transform.localScale =
                    Vector3.one * 0.2f;

                speed = 1.5f;

                break;

            case AsteroidSize.Pequeno:

                transform.localScale =
                    Vector3.one * 0.2f;

                speed = 1f;

                break;
        }
    }

    public AsteroidSize GetSize()
    {
        return size;
    }
}