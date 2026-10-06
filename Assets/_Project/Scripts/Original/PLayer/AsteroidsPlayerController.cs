using UnityEngine;
using UnityEngine.InputSystem;

public class AsteroidsPlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference fireAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Camera")]
    [SerializeField] private Camera asteroidsCamera;

    [SerializeField] private OriginalAsteroidsPlayerHealth playerHealth;

    private Vector2 movementInput;
    private Vector2 mouseInput;

    private float nextFireTime;

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

        if (asteroidsCamera == null)
        {
            Debug.LogError(
                "AsteroidsPlayerController: " +
                "AsteroidsCamera não foi encontrada!"
            );
        }

        if (playerHealth == null)
        {
            playerHealth =
                GetComponent<OriginalAsteroidsPlayerHealth>();
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();

        if (lookAction != null)
            lookAction.action.Enable();

        if (fireAction != null)
            fireAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();

        if (lookAction != null)
            lookAction.action.Disable();

        if (fireAction != null)
            fireAction.action.Disable();
    }

    private void Update()
    {
        if (playerHealth != null &&
            (playerHealth.IsGameOver() ||
             playerHealth.IsGameFinished()))
        {
            return;
        }

        if (moveAction != null)
            movementInput = moveAction.action.ReadValue<Vector2>();

        if (lookAction != null)
            mouseInput = lookAction.action.ReadValue<Vector2>();

        Move();
        Rotate();
        Shoot();
        WrapAround();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(
            movementInput.x,
            movementInput.y,
            0f
        );

        // Evita que o movimento diagonal fique mais rápido
        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        transform.position +=
            movement *
            moveSpeed *
            Time.deltaTime;
    }

    private void Rotate()
    {
        float rotation =
            -mouseInput.x *
            rotationSpeed *
            Time.deltaTime;

        transform.Rotate(
            Vector3.forward,
            rotation,
            Space.Self
        );
    }

    private void Shoot()
    {
        if (playerHealth != null &&
            playerHealth.IsGameOver())
        {
            return;
        }

        if (fireAction == null)
            return;

        if (!fireAction.action.IsPressed())
            return;

        if (Time.time < nextFireTime)
            return;

        if (bulletPrefab == null || firePoint == null)
            return;

        nextFireTime =
            Time.time +
            fireRate;

        Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );
    }

    private void WrapAround()
    {
        if (asteroidsCamera == null)
            return;

        // Pega a posição do Player
        Vector3 position = transform.position;

        // Converte a posição do Player para viewport.
        // X e Y ficam entre 0 e 1.
        Vector3 viewportPosition =
            asteroidsCamera.WorldToViewportPoint(position);

        bool changed = false;

        // Saiu pela direita
        if (viewportPosition.x > 1f)
        {
            viewportPosition.x = 0f;
            changed = true;
        }

        // Saiu pela esquerda
        else if (viewportPosition.x < 0f)
        {
            viewportPosition.x = 1f;
            changed = true;
        }

        // Saiu por cima
        if (viewportPosition.y > 1f)
        {
            viewportPosition.y = 0f;
            changed = true;
        }

        // Saiu por baixo
        else if (viewportPosition.y < 0f)
        {
            viewportPosition.y = 1f;
            changed = true;
        }

        // Se saiu da tela, converte novamente
        // para coordenadas do mundo.
        if (changed)
        {
            position =
                asteroidsCamera.ViewportToWorldPoint(
                    viewportPosition
                );

            // Mantém a profundidade original
            position.z = transform.position.z;

            transform.position = position;
        }
    }
}