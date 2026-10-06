using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Player - Vida Extra")]
    [SerializeField] private int pointsPerExtraLife = 300;

    [Header("Estação - Recuperação de Vida")]
    [SerializeField] private int pointsPerStationHeal = 300;
    [SerializeField] private int stationHealAmount = 100;

    private int nextLifeScore = 300;
    private int nextStationHealScore = 300;

    private int currentScore;
    private int highScore;

    public int CurrentScore
    {
        get { return currentScore; }
    }

    private const string HIGH_SCORE_KEY = "HIGH_SCORE";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        UpdateScoreUI();
        UpdateHighScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;

        UpdateScoreUI();

        // Vida extra do Player
        CheckExtraLife();

        // Recuperação de vida da Estação
        CheckStationHeal();
    }

    private void CheckExtraLife()
    {
        while (currentScore >= nextLifeScore)
        {
            PlayerHealth playerHealth =
                FindObjectOfType<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(1);

                Debug.Log(
                    "Vida extra conquistada aos " +
                    nextLifeScore +
                    " pontos!"
                );
            }

            nextLifeScore += pointsPerExtraLife;
        }
    }

    private void CheckStationHeal()
    {
        while (currentScore >= nextStationHealScore)
        {
            StationHealth stationHealth =
                FindObjectOfType<StationHealth>();

            if (stationHealth != null)
            {
                stationHealth.Heal(stationHealAmount);

                Debug.Log(
                    "A Estação recuperou " +
                    stationHealAmount +
                    " de vida aos " +
                    nextStationHealScore +
                    " pontos!"
                );
            }

            nextStationHealScore += pointsPerStationHeal;
        }
    }

    public void ResetScore()
    {
        currentScore = 0;

        // Reinicia os próximos marcos de recompensa
        nextLifeScore = pointsPerExtraLife;
        nextStationHealScore = pointsPerStationHeal;

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + currentScore;
    }

    private void UpdateHighScoreUI()
    {
        highScoreText.text = "High Score: " + highScore;
    }
}