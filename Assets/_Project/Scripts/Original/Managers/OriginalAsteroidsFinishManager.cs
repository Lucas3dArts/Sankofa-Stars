using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OriginalAsteroidsFinishManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private GameObject cutscenePanel;

    [Header("Cutscene")]
    [SerializeField] private float cutsceneDuration = 5f;

    private bool finished = false;

    public void FinishMiniGame()
    {
        finished = true;

        if (finishPanel != null)
        {
            finishPanel.SetActive(true);
        }

        Debug.Log("Mini-Asteroids concluído! Aguardando tecla...");
    }

    private void Update()
    {
        if (!finished)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.anyKey.wasPressedThisFrame)
        {
            Continue();
        }
    }

    private void Continue()
    {
        Debug.Log("Iniciando cutscene...");

        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
        }

        if (cutscenePanel != null)
        {
            cutscenePanel.SetActive(true);
        }

        Invoke(nameof(EndCutscene), cutsceneDuration);
    }

    private void EndCutscene()
    {
        Debug.Log("Cutscene terminou! Entrando no jogo 3D...");

        Time.timeScale = 1f;
        SceneManager.LoadScene("Jogo");
    }
}