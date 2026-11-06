using UnityEngine;
using UnityEngine.InputSystem;

public class AsteroidsPlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    private InputActionReference moveAction;

    [Tooltip(
        "Configure como Pass Through, Vector2 e <Pointer>/position."
    )]
    [SerializeField]
    private InputActionReference lookAction;

    [SerializeField]
    private InputActionReference fireAction;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5f;

    [Header("Rotation")]
    [Tooltip("Velocidade de rotação em graus por segundo.")]
    [SerializeField]
    private float rotationSpeed = 720f;

    [Tooltip(
        "Correção da frente visual da nave. " +
        "Teste 0, -90, 90 ou 180."
    )]
    [SerializeField]
    private float rotationOffset = -90f;

    [Tooltip(
        "Quando desmarcado, a nave aponta imediatamente para o mouse."
    )]
    [SerializeField]
    private bool smoothRotation = true;

    [Tooltip(
        "Evita instabilidade quando o mouse está muito perto da nave."
    )]
    [SerializeField]
    private float minimumLookDistance = 0.05f;

    [Header("Shooting")]
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private float fireRate = 0.2f;

    [Header("Camera")]
    [SerializeField]
    private Camera asteroidsCamera;

    [Header("Wrap Around")]
    [Tooltip(
        "0 usa toda a largura. " +
        "0.20 utiliza somente os 60% centrais."
    )]
    [Range(0f, 0.49f)]
    [SerializeField]
    private float horizontalWrapMargin = 0.20f;

    [Tooltip(
        "0 usa toda a altura. " +
        "0.25 utiliza somente os 50% centrais."
    )]
    [Range(0f, 0.49f)]
    [SerializeField]
    private float verticalWrapMargin = 0.25f;

    [Tooltip(
        "Coloca o jogador um pouco para dentro " +
        "depois do teletransporte."
    )]
    [Range(0f, 0.1f)]
    [SerializeField]
    private float wrapPadding = 0.01f;

    [Header("Cursor")]
    [Tooltip(
        "Esconde o cursor enquanto o minigame está ativo."
    )]
    [SerializeField]
    private bool hideCursor = true;

    [Tooltip(
        "Mantém o cursor dentro da janela do jogo."
    )]
    [SerializeField]
    private bool confineCursor = true;

    [Tooltip(
        "Mostra e libera o cursor durante o Pause."
    )]
    [SerializeField]
    private bool showCursorDuringPause = true;

    [Header("Pause")]
    [Tooltip(
        "Considera o jogo pausado quando Time.timeScale é igual a zero."
    )]
    [SerializeField]
    private bool detectPauseByTimeScale = true;

    [Header("Health")]
    [SerializeField]
    private OriginalAsteroidsPlayerHealth playerHealth;

    private Vector2 movementInput;
    private Vector2 mouseScreenPosition;

    private float nextFireTime;

    private bool wasPaused;
    private bool cursorWasRestored;
    private bool applicationHasFocus = true;

    private void Awake()
    {
        FindAsteroidsCamera();

        if (playerHealth == null)
        {
            playerHealth =
                GetComponent<OriginalAsteroidsPlayerHealth>();
        }

        wasPaused = IsPaused();
    }

    private void Start()
    {
        if (!IsPaused())
        {
            ReactivateInputActions();
            ApplyGameplayCursorSettings();
        }
    }

    private void OnEnable()
    {
        cursorWasRestored = false;
        wasPaused = IsPaused();

        ReactivateInputActions();

        if (!wasPaused)
        {
            ApplyGameplayCursorSettings();
        }
    }

    private void OnDisable()
    {
        DisableInputActions();
        RestoreCursor();
    }

    private void OnDestroy()
    {
        RestoreCursor();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        applicationHasFocus = hasFocus;

        if (!hasFocus)
        {
            RestoreCursor();
            return;
        }

        if (!IsPaused() && !GameHasEnded())
        {
            ReactivateInputActions();
            RefreshMousePosition();
            ApplyGameplayCursorSettings();
        }
    }

    private void Update()
    {
        HandlePauseState();

        if (IsPaused())
            return;

        if (GameHasEnded())
        {
            if (!cursorWasRestored)
            {
                RestoreCursor();
                cursorWasRestored = true;
            }

            return;
        }

        if (!applicationHasFocus)
            return;

        EnsureInputActionsAreEnabled();
        ReadInput();

        Move();
        RotateTowardsMouse();
        Shoot();
        WrapAround();
    }

    private bool IsPaused()
    {
        if (!detectPauseByTimeScale)
            return false;

        return Mathf.Approximately(
            Time.timeScale,
            0f
        );
    }

    private void HandlePauseState()
    {
        bool isPausedNow = IsPaused();

        /*
         * Entrou no Pause.
         */
        if (isPausedNow && !wasPaused)
        {
            wasPaused = true;

            movementInput = Vector2.zero;

            if (showCursorDuringPause)
            {
                RestoreCursor();
            }

            return;
        }

        /*
         * Saiu do Pause.
         */
        if (!isPausedNow && wasPaused)
        {
            wasPaused = false;
            cursorWasRestored = false;

            ReactivateInputActions();
            RefreshMousePosition();
            ApplyGameplayCursorSettings();
        }
    }

    private bool GameHasEnded()
    {
        if (playerHealth == null)
            return false;

        return
            playerHealth.IsGameOver() ||
            playerHealth.IsGameFinished();
    }

    private void ReactivateInputActions()
    {
        /*
         * Reativamos explicitamente cada ação.
         *
         * Isso resolve o caso em que o menu de Pause
         * desativa o Action Map ou alguma ação individual.
         */
        if (moveAction != null)
        {
            if (!moveAction.action.enabled)
            {
                moveAction.action.Enable();
            }
        }

        if (lookAction != null)
        {
            if (!lookAction.action.enabled)
            {
                lookAction.action.Enable();
            }
        }

        if (fireAction != null)
        {
            if (!fireAction.action.enabled)
            {
                fireAction.action.Enable();
            }
        }
    }

    private void EnsureInputActionsAreEnabled()
    {
        /*
         * Verificação adicional caso outro script desative
         * as ações depois do primeiro frame do Resume.
         */
        if (moveAction != null &&
            !moveAction.action.enabled)
        {
            moveAction.action.Enable();
        }

        if (lookAction != null &&
            !lookAction.action.enabled)
        {
            lookAction.action.Enable();
        }

        if (fireAction != null &&
            !fireAction.action.enabled)
        {
            fireAction.action.Enable();
        }
    }

    private void DisableInputActions()
    {
        if (moveAction != null &&
            moveAction.action.enabled)
        {
            moveAction.action.Disable();
        }

        if (lookAction != null &&
            lookAction.action.enabled)
        {
            lookAction.action.Disable();
        }

        if (fireAction != null &&
            fireAction.action.enabled)
        {
            fireAction.action.Disable();
        }
    }

    private void FindAsteroidsCamera()
    {
        if (asteroidsCamera != null)
            return;

        GameObject cameraObject =
            GameObject.Find("AsteroidsCamera");

        if (cameraObject != null)
        {
            asteroidsCamera =
                cameraObject.GetComponent<Camera>();
        }

        if (asteroidsCamera == null)
        {
            Debug.LogError(
                "AsteroidsPlayerController: " +
                "a AsteroidsCamera não foi encontrada."
            );
        }
    }

    private void ReadInput()
    {
        movementInput = Vector2.zero;

        if (moveAction != null &&
            moveAction.action.enabled)
        {
            movementInput =
                moveAction.action.ReadValue<Vector2>();
        }

        if (lookAction != null &&
            lookAction.action.enabled)
        {
            mouseScreenPosition =
                lookAction.action.ReadValue<Vector2>();
        }
    }

    private void RefreshMousePosition()
    {
        /*
         * Força uma nova leitura da posição do mouse
         * assim que o jogo volta do Pause.
         */
        if (lookAction == null)
            return;

        if (!lookAction.action.enabled)
        {
            lookAction.action.Enable();
        }

        mouseScreenPosition =
            lookAction.action.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(
            movementInput.x,
            movementInput.y,
            0f
        );

        /*
         * Impede que o movimento diagonal
         * fique mais rápido.
         */
        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        transform.position +=
            movement *
            moveSpeed *
            Time.deltaTime;
    }

    private void RotateTowardsMouse()
    {
        if (asteroidsCamera == null)
            return;

        if (lookAction == null)
            return;

        if (!lookAction.action.enabled)
            return;

        /*
         * A ação Look retorna a posição absoluta
         * do ponteiro em pixels.
         *
         * O binding deve ser:
         * <Pointer>/position
         */
        float viewportX =
            mouseScreenPosition.x /
            Mathf.Max(1f, Screen.width);

        float viewportY =
            mouseScreenPosition.y /
            Mathf.Max(1f, Screen.height);

        /*
         * Mantém os valores dentro da área válida
         * do Viewport.
         */
        viewportX =
            Mathf.Clamp01(viewportX);

        viewportY =
            Mathf.Clamp01(viewportY);

        float distanceFromCamera =
            Mathf.Abs(
                transform.position.z -
                asteroidsCamera.transform.position.z
            );

        Vector3 mouseViewportPosition =
            new Vector3(
                viewportX,
                viewportY,
                distanceFromCamera
            );

        Vector3 mouseWorldPosition =
            asteroidsCamera.ViewportToWorldPoint(
                mouseViewportPosition
            );

        Vector2 direction = new Vector2(
            mouseWorldPosition.x -
            transform.position.x,

            mouseWorldPosition.y -
            transform.position.y
        );

        float minimumDistanceSquared =
            minimumLookDistance *
            minimumLookDistance;

        if (direction.sqrMagnitude <
            minimumDistanceSquared)
        {
            return;
        }

        float targetAngle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;

        targetAngle +=
            rotationOffset;

        Quaternion targetRotation =
            Quaternion.Euler(
                0f,
                0f,
                targetAngle
            );

        if (smoothRotation)
        {
            /*
             * unscaledDeltaTime não fica zerado
             * durante a transição do Pause.
             */
            transform.rotation =
                Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.unscaledDeltaTime
                );
        }
        else
        {
            transform.rotation =
                targetRotation;
        }
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

        if (!fireAction.action.enabled)
            return;

        if (!fireAction.action.IsPressed())
            return;

        if (Time.time < nextFireTime)
            return;

        if (bulletPrefab == null ||
            firePoint == null)
        {
            return;
        }

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

        Vector3 originalPosition =
            transform.position;

        Vector3 viewportPosition =
            asteroidsCamera.WorldToViewportPoint(
                originalPosition
            );

        float leftLimit =
            horizontalWrapMargin;

        float rightLimit =
            1f - horizontalWrapMargin;

        float bottomLimit =
            verticalWrapMargin;

        float topLimit =
            1f - verticalWrapMargin;

        bool positionChanged = false;

        /*
         * Saiu pela direita.
         * Reaparece pela esquerda.
         */
        if (viewportPosition.x >
            rightLimit + wrapPadding)
        {
            viewportPosition.x =
                leftLimit + wrapPadding;

            positionChanged = true;
        }
        /*
         * Saiu pela esquerda.
         * Reaparece pela direita.
         */
        else if (viewportPosition.x <
                 leftLimit - wrapPadding)
        {
            viewportPosition.x =
                rightLimit - wrapPadding;

            positionChanged = true;
        }

        /*
         * Saiu por cima.
         * Reaparece por baixo.
         */
        if (viewportPosition.y >
            topLimit + wrapPadding)
        {
            viewportPosition.y =
                bottomLimit + wrapPadding;

            positionChanged = true;
        }
        /*
         * Saiu por baixo.
         * Reaparece por cima.
         */
        else if (viewportPosition.y <
                 bottomLimit - wrapPadding)
        {
            viewportPosition.y =
                topLimit - wrapPadding;

            positionChanged = true;
        }

        if (!positionChanged)
            return;

        Vector3 correctedPosition =
            asteroidsCamera.ViewportToWorldPoint(
                viewportPosition
            );

        /*
         * Mantém o jogador no mesmo plano Z.
         */
        correctedPosition.z =
            originalPosition.z;

        transform.position =
            correctedPosition;
    }

    private void ApplyGameplayCursorSettings()
    {
        Cursor.visible =
            !hideCursor;

        Cursor.lockState =
            confineCursor
                ? CursorLockMode.Confined
                : CursorLockMode.None;
    }

    private void RestoreCursor()
    {
        Cursor.visible = true;

        Cursor.lockState =
            CursorLockMode.None;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        moveSpeed =
            Mathf.Max(
                0f,
                moveSpeed
            );

        rotationSpeed =
            Mathf.Max(
                0f,
                rotationSpeed
            );

        minimumLookDistance =
            Mathf.Max(
                0f,
                minimumLookDistance
            );

        fireRate =
            Mathf.Max(
                0.01f,
                fireRate
            );

        horizontalWrapMargin =
            Mathf.Clamp(
                horizontalWrapMargin,
                0f,
                0.49f
            );

        verticalWrapMargin =
            Mathf.Clamp(
                verticalWrapMargin,
                0f,
                0.49f
            );

        wrapPadding =
            Mathf.Clamp(
                wrapPadding,
                0f,
                0.1f
            );
    }

#endif
}