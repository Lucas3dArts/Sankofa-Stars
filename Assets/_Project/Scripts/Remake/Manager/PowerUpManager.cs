using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform station;

    [Header("Power-Up Prefabs")]
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject freezePrefab;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject lightningPrefab;

    [Header("Spawn Position")]
    [SerializeField] private float minSpawnDistance = 40f;
    [SerializeField] private float maxSpawnDistance = 100f;

    [Header("Base Spawn")]
    [SerializeField] private float initialSpawnInterval = 45f;

    [Header("Score Scaling")]
    [SerializeField] private int scoreStep = 300;
    [SerializeField] private float intervalReduction = 5f;
    [SerializeField] private float minimumSpawnInterval = 10f;

    [Header("Power-Up Limit")]
    [SerializeField] private int maxPowerUpsOnScreen = 2;

    private float nextSpawnTime;

    private void Start()
    {
        ScheduleNextSpawn();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnPowerUp();

            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        float currentInterval =
            CalculateSpawnInterval();

        nextSpawnTime =
            Time.time + currentInterval;

        Debug.Log(
            "Próximo Power-Up em aproximadamente: " +
            currentInterval +
            " segundos."
        );
    }

    private float CalculateSpawnInterval()
    {
        int currentScore = 0;

        if (HighScoreManager.Instance != null)
        {
            currentScore =
                HighScoreManager.Instance.CurrentScore;
        }

        int scoreSteps =
            currentScore / scoreStep;

        float interval =
            initialSpawnInterval -
            (scoreSteps * intervalReduction);

        interval =
            Mathf.Max(
                interval,
                minimumSpawnInterval
            );

        return interval;
    }

    private void TrySpawnPowerUp()
    {
        if (station == null)
        {
            Debug.LogError(
                "PowerUpManager: Station não definida!"
            );

            return;
        }

        int currentPowerUps =
            GameObject.FindGameObjectsWithTag(
                "PowerUp"
            ).Length;

        if (currentPowerUps >= maxPowerUpsOnScreen)
        {
            Debug.Log(
                "Limite de Power-Ups na tela atingido."
            );

            return;
        }

        GameObject selectedPrefab =
            SelectRandomPowerUp();

        if (selectedPrefab == null)
        {
            Debug.LogError(
                "PowerUpManager: " +
                "Nenhum Power-Up foi configurado!"
            );

            return;
        }

        SpawnPowerUp(selectedPrefab);
    }

    private GameObject SelectRandomPowerUp()
    {
        GameObject[] powerUps =
        {
            shieldPrefab,
            freezePrefab,
            missilePrefab,
            lightningPrefab
        };

        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            int index =
                Random.Range(
                    0,
                    powerUps.Length
                );

            if (powerUps[index] != null)
                return powerUps[index];
        }

        return null;
    }

    private void SpawnPowerUp(
        GameObject powerUpPrefab
    )
    {
        Vector3 direction =
            Random.onUnitSphere;

        Vector3 spawnPosition =
            station.position +
            direction *
            Random.Range(
                minSpawnDistance,
                maxSpawnDistance
            );

        GameObject powerUp =
            Instantiate(
                powerUpPrefab,
                spawnPosition,
                Random.rotation
            );

        Debug.Log(
            "⚡ Power-Up apareceu: " +
            powerUp.name
        );
    }
}