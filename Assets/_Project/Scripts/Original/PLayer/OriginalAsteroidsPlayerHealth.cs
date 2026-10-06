using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class OriginalAsteroidsPlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxLives = 3;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityTime = 2f;

    [Header("UI")]
    [SerializeField] private TMP_Text livesText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private int currentLives;

    private Vector3 startingPosition;

    private bool invulnerable = false;

    private float invulnerabilityTimer;

    private bool gameFinished = false;

    private void Start()
    {
        currentLives = maxLives;

        startingPosition = transform.position;

        UpdateLivesUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Debug.Log(
            "Vidas do Asteroids: " +
            currentLives
        );
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;

            if (invulnerabilityTimer <= 0f)
            {
                invulnerable = false;

                Debug.Log(
                    "Invulnerabilidade terminou!"
                );
            }
        }

        if (currentLives <= 0)
        {
            if (Keyboard.current != null &&
                Keyboard.current.anyKey.wasPressedThisFrame)
            {
                RestartGame();
            }
        }
    }

    public void TakeDamage()
    {
        if (currentLives <= 0)
            return;

        if (invulnerable)
            return;

        currentLives--;
        UpdateLivesUI();

        Debug.Log(
            "Player atingido! Vidas restantes: " +
            currentLives
        );

        if (currentLives <= 0)
        {
            Die();
            return;
        }

        Respawn();
    }

    private void Respawn()
    {
        transform.position = startingPosition;

        invulnerable = true;
        invulnerabilityTimer = invulnerabilityTime;

        Debug.Log(
            "Player respawnou! " +
            invulnerabilityTime +
            " segundos de invulnerabilidade."
        );
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text =
                "VIDAS: " +
                currentLives;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroide"))
        {
            TakeDamage();

            Destroy(other.gameObject);
        }
    }

    private void Die()
    {
        Debug.Log("GAME OVER - Asteroids!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        UnityEngine.SceneManagement.Scene currentScene =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            currentScene.buildIndex
        );
    }

    public void FinishGame()
    {
        gameFinished = true;

        Debug.Log("Mini-Asteroids concluído!");
    }

    public int GetLives()
    {
        return currentLives;
    }

    public bool IsGameOver()
    {
        return currentLives <= 0;
    }

    public bool IsGameFinished()
    {
        return gameFinished;
    }
}