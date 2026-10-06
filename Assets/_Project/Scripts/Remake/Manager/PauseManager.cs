using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);

        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        controlsPanel.SetActive(false);

        Time.timeScale = 0f;

        isPaused = true;

        UnlockCursor();
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;

        LockCursor();
    }

    // =========================
    // MUDAR TECLAS
    // =========================

    public void OpenControls()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);

        UnlockCursor();
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);

        UnlockCursor();
    }

    // =========================
    // REINICIAR
    // =========================

    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // =========================
    // MENU PRINCIPAL
    // =========================

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Main Menu");
    }

    // =========================
    // OPÇÕES
    // =========================

    public void Options()
    {
        Debug.Log("Abrir opções futuramente");
    }

    // =========================
    // CURSOR
    // =========================

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}