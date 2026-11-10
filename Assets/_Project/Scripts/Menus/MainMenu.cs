using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject optionsPanel;

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
        optionsPanel.SetActive(false);
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
        optionsPanel.SetActive(false);
    }

    public void CloseHowToPlay()
    {
        ShowMainMenu();
    }

    // =========================
    // CONTROLES
    // =========================

    public void OpenControls()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        controlsPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    // =========================
    // OPÇÕES
    // =========================

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        ShowMainMenu();
    }
}