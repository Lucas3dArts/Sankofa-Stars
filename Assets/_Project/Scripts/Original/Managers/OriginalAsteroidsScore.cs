using UnityEngine;
using TMPro;

public class OriginalAsteroidsScore : MonoBehaviour
{
    public static OriginalAsteroidsScore Instance;

    [Header("Score")]
    [SerializeField] private int score = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Final do Mini-Asteroids")]
    [SerializeField] private int scoreToFinish = 500;
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private OriginalAsteroidsFinishManager finishManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;

        UpdateUI();

        Debug.Log(
            "Score do Asteroids: " +
            score
        );

        if (score >= scoreToFinish)
        {
            FinishMiniGame();
        }
    }

    private void FinishMiniGame()
    {
        Debug.Log("Mini-Asteroids concluído!");

        OriginalAsteroidsPlayerHealth playerHealth =
            FindObjectOfType<OriginalAsteroidsPlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.FinishGame();
        }

        if (finishManager != null)
        {
            finishManager.FinishMiniGame();
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }

    public int GetScore()
    {
        return score;
    }
}