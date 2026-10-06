using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject controlsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    // =========================
    // MENU PRINCIPAL
    // =========================

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    // =========================
    // JOGAR
    // =========================

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("OriginalAsteroids");
    }

    // =========================
    // SAIR
    // =========================

    public void QuitGame()
    {
        Application.Quit();
    }

    // =========================
    // COMO JOGAR
    // =========================

    public void OpenHowToPlay()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void CloseHowToPlay()
    {
        ShowMainMenu();
    }

    // =========================
    // MUDAR TECLAS
    // =========================

    public void OpenControls()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
}