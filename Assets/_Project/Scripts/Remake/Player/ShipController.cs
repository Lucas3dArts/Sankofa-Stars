using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference rollAction;
    [SerializeField] private InputActionReference powerUpAction;
    [SerializeField] private InputActionReference moveSpeedAction;

    [Header("Movement Mode")]
    [SerializeField] private MovementMode movementMode = MovementMode.WS;

    [Header("Movement")]
    [SerializeField] private float thrustForce = 50f;
    [SerializeField] private float strafeForce = 25f;
    [SerializeField] private float scrollSensitivity = 0.01f;

    [Header("Rotation")]
    [SerializeField] private float pitchSpeed = 2f;
    [SerializeField] private float yawSpeed = 2f;
    [SerializeField] private float rollSpeed = 100f;

    [Header("Mouse Sensitivity")]
    [SerializeField] private float mouseSensitivity = 0.05f;

    [Header("Iansã - Velocidade da Tempestade")]
    [SerializeField] private float stormSpeedMultiplier = 2f;
    [SerializeField] private float stormRotationMultiplier = 1.5f;

    private Vector2 movementInput;
    private Vector2 mouseInput;
    private float rollInput;
    private float scrollSpeed;

    private bool stormAvailable = false;
    private bool stormActive = false;
    private float stormDuration;

    private const string MOVEMENT_MODE_KEY = "MOVEMENT_MODE";

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError(
                "ShipController: Nenhum Rigidbody encontrado no Player!"
            );

            return;
        }

        LoadMovementMode();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();

        if (lookAction != null)
            lookAction.action.Enable();

        if (rollAction != null)
            rollAction.action.Enable();

        if (powerUpAction != null)
            powerUpAction.action.Enable();

        if (moveSpeedAction != null)
            moveSpeedAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();

        if (lookAction != null)
            lookAction.action.Disable();

        if (rollAction != null)
            rollAction.action.Disable();

        if (powerUpAction != null)
            powerUpAction.action.Disable();

        if (moveSpeedAction != null)
            moveSpeedAction.action.Disable();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        HandleMovement();
        HandleRotation();
    }
    public enum MovementMode
    {
        WS,
        Scroll
    }

    private void LoadMovementMode()
    {
        if (!PlayerPrefs.HasKey(MOVEMENT_MODE_KEY))
        {
            movementMode = MovementMode.WS;
            return;
        }

        int savedMode =
            PlayerPrefs.GetInt(
                MOVEMENT_MODE_KEY
            );

        movementMode =
            (MovementMode)savedMode;

        scrollSpeed = 0f;

        Debug.Log(
            "Modo de movimento carregado: " +
            movementMode
        );
    }

    public MovementMode CurrentMovementMode
    {
        get
        {
            return movementMode;
        }
    }
    public void SetMovementMode(MovementMode mode)
    {
        movementMode = mode;

        // Zera o scroll ao trocar de modo
        scrollSpeed = 0f;

        Debug.Log("Modo de movimento: " + movementMode);
    }
    private void ReadInput()
    {
        // =========================
        // MOVIMENTO W / S / A / D
        // =========================

        if (moveAction != null)
        {
            movementInput =
                moveAction.action.ReadValue<Vector2>();
        }
        else
        {
            movementInput = Vector2.zero;
        }

        // =========================
        // MOUSE
        // =========================

        if (lookAction != null)
        {
            mouseInput =
                lookAction.action.ReadValue<Vector2>();
        }
        else
        {
            mouseInput = Vector2.zero;
        }

        // =========================
        // ROLL Q / E
        // =========================

        if (rollAction != null)
        {
            rollInput =
                rollAction.action.ReadValue<float>();
        }
        else
        {
            rollInput = 0f;
        }

        // =========================
        // SCROLL
        // =========================

        if (movementMode == MovementMode.Scroll &&
            moveSpeedAction != null)
        {
            float scroll =
                moveSpeedAction.action.ReadValue<float>();

            if (Mathf.Abs(scroll) > 0.01f)
            {
                scrollSpeed +=
                    scroll * scrollSensitivity;
            }

            scrollSpeed =
                Mathf.Clamp(
                    scrollSpeed,
                    -1f,
                    1f
                );
        }

        // =========================
        // SE ESTIVER USANDO W/S
        // =========================

        if (movementMode == MovementMode.WS)
        {
            scrollSpeed = 0f;
        }
    }

    private void HandleMovement()
    {
        float currentThrust =
            thrustForce;

        float currentStrafe =
            strafeForce;

        // =========================
        // TEMPESTADE DE IANSÃ
        // =========================

        if (stormActive)
        {
            currentThrust *=
                stormSpeedMultiplier;

            currentStrafe *=
                stormSpeedMultiplier;
        }

        // =========================
        // FRENTE / TRÁS
        // =========================

        float forwardInput = 0f;

        if (movementMode == MovementMode.WS)
        {
            // W / S
            forwardInput =
                movementInput.y;
        }
        else if (movementMode == MovementMode.Scroll)
        {
            // Scroll
            forwardInput =
                scrollSpeed;
        }

        Vector3 forwardMovement =
            transform.forward *
            forwardInput *
            currentThrust;

        // =========================
        // ESQUERDA / DIREITA
        // A / D
        // =========================

        Vector3 strafeMovement =
            transform.right *
            movementInput.x *
            currentStrafe;

        // =========================
        // MOVIMENTO FINAL
        // =========================

        Vector3 movement =
            forwardMovement +
            strafeMovement;

        rb.AddForce(
            movement,
            ForceMode.Acceleration
        );
    }

    private void HandleRotation()
    {
        float currentPitchSpeed =
            pitchSpeed;

        float currentYawSpeed =
            yawSpeed;

        float currentRollSpeed =
            rollSpeed;

        // Tempestade aumenta a rotação
        if (stormActive)
        {
            currentPitchSpeed *=
                stormRotationMultiplier;

            currentYawSpeed *=
                stormRotationMultiplier;

            currentRollSpeed *=
                stormRotationMultiplier;
        }

        // =========================
        // YAW
        // Mouse X
        // =========================

        float yaw =
            mouseInput.x *
            currentYawSpeed *
            mouseSensitivity *
            Time.fixedDeltaTime;

        // =========================
        // PITCH
        // Mouse Y
        // =========================

        float pitch =
            -mouseInput.y *
            currentPitchSpeed *
            mouseSensitivity *
            Time.fixedDeltaTime;

        // =========================
        // ROLL
        // Q / E
        // =========================

        float roll =
            rollInput *
            currentRollSpeed *
            Time.fixedDeltaTime;

        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                roll
            );

        rb.MoveRotation(
            rb.rotation *
            rotation
        );
    }

    // ==========================================
    // IANSÃ
    // ==========================================

    public void StoreStormPowerUp(float duration)
    {
        stormAvailable = true;
        stormDuration = duration;

        Debug.Log(
            "⚡ VELOCIDADE DA TEMPESTADE DISPONÍVEL!"
        );

        Debug.Log(
            "Pressione o BOTÃO DIREITO para ativar."
        );
    }

    public void ActivateStorm(float duration)
    {
        stormAvailable = false;
        stormActive = true;
        stormDuration = duration;

        CancelInvoke(
            nameof(DeactivateStorm)
        );

        Invoke(
            nameof(DeactivateStorm),
            stormDuration
        );

        Debug.Log(
            "⚡ VELOCIDADE DA TEMPESTADE ATIVADA!"
        );

        Debug.Log(
            "Multiplicador de velocidade: " +
            stormSpeedMultiplier
        );
    }

    private void DeactivateStorm()
    {
        stormActive = false;

        Debug.Log(
            "⚡ VELOCIDADE DA TEMPESTADE TERMINOU!"
        );
    }

    public bool IsStormActive
    {
        get
        {
            return stormActive;
        }
    }

    public bool HasStormPowerUp
    {
        get
        {
            return stormAvailable;
        }
    }
}