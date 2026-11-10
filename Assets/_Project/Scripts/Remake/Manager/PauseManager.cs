using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject optionsPanel;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);

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

    // =========================
    // PAUSE
    // =========================

    public void Pause()
    {
        pausePanel.SetActive(true);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;

        UnlockCursor();
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        LockCursor();
    }

    // =========================
    // CONTROLES
    // =========================

    public void OpenControls()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
        optionsPanel.SetActive(false);

        UnlockCursor();
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);

        UnlockCursor();
    }

    // =========================
    // OPÇÕES
    // =========================

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(true);

        UnlockCursor();
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
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
    // CURSOR
    // =========================

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}