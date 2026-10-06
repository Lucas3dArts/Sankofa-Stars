using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;

    private bool gameOver = false;

    private void Awake()
    {
        // Garante que o jogo nunca comece pausado
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        gameOver = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!gameOver)
            return;

        if (Input.anyKeyDown)
        {
            RestartGame();
        }
    }

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("=== GAME OVER ===");
        Debug.Log("Pressione qualquer tecla para reiniciar.");
    }

    public void RestartGame()
    {
        Debug.Log("=== REINICIANDO PARTIDA ===");

        // MUITO IMPORTANTE:
        // restaura o tempo antes de carregar a cena
        Time.timeScale = 1f;

        // Garante que o estado do jogo seja limpo
        gameOver = false;

        // Esconde o cursor antes de carregar a nova partida
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}