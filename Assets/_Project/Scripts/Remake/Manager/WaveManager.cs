using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private Transform station;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 100f;

    [Header("Wave Settings")]
    [SerializeField] private int initialAsteroids = 5;
    [SerializeField] private float waveDuration = 30f;
    [SerializeField] private float delayBetweenWaves = 2f;

    [Header("Asteroid Force")]
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 10f;

    [Header("Score Difficulty")]
    [SerializeField] private int scoreStep = 300;
    [SerializeField] private int asteroidsIncreasePerScore = 2;
    [SerializeField] private int maximumAsteroidsPerWave = 30;

    private int currentWave = 0;

    private void Start()
    {
        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            currentWave++;

            int asteroidsToSpawn =
                CalculateAsteroidsForWave();

            Debug.Log(
                "=== WAVE " +
                currentWave +
                " ==="
            );

            Debug.Log(
                "Asteroides nesta wave: " +
                asteroidsToSpawn
            );

            SpawnWave(asteroidsToSpawn);

            float timer = 0f;

            while (
                GameObject.FindGameObjectsWithTag("Asteroide").Length > 0
                &&
                timer < waveDuration
            )
            {
                timer += Time.deltaTime;

                yield return null;
            }

            Debug.Log(
                "Fim da Wave " +
                currentWave
            );

            yield return new WaitForSeconds(
                delayBetweenWaves
            );
        }
    }

    private int CalculateAsteroidsForWave()
    {
        int currentScore = 0;

        if (HighScoreManager.Instance != null)
        {
            currentScore =
                HighScoreManager.Instance.CurrentScore;
        }

        int scoreSteps =
            currentScore / scoreStep;

        int asteroidsToSpawn =
            initialAsteroids +
            (scoreSteps * asteroidsIncreasePerScore);

        asteroidsToSpawn =
            Mathf.Min(
                asteroidsToSpawn,
                maximumAsteroidsPerWave
            );

        return asteroidsToSpawn;
    }

    private void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        if (asteroidPrefab == null)
        {
            Debug.LogError(
                "WaveManager: Asteroid Prefab não foi definido!"
            );

            return;
        }

        if (station == null)
        {
            Debug.LogError(
                "WaveManager: Station não foi definida!"
            );

            return;
        }

        Vector3 spawnPos =
            GetRandomPointOnSphere();

        GameObject asteroid =
            Instantiate(
                asteroidPrefab,
                spawnPos,
                Random.rotation
            );

        Rigidbody rb =
            asteroid.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction =
                (station.position -
                 asteroid.transform.position)
                .normalized;

            float force =
                Random.Range(
                    minForce,
                    maxForce
                );

            rb.AddForce(
                direction * force,
                ForceMode.Impulse
            );

            rb.AddTorque(
                Random.onUnitSphere * 50f,
                ForceMode.Impulse
            );
        }

        IgnoreAsteroidCollisions(asteroid);
    }

    private void IgnoreAsteroidCollisions(
        GameObject asteroid
    )
    {
        Collider asteroidCollider =
            asteroid.GetComponent<Collider>();

        if (asteroidCollider == null)
            return;

        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag(
                "Asteroide"
            );

        foreach (GameObject other in asteroids)
        {
            if (other == asteroid)
                continue;

            Collider otherCollider =
                other.GetComponent<Collider>();

            if (otherCollider != null)
            {
                Physics.IgnoreCollision(
                    asteroidCollider,
                    otherCollider
                );
            }
        }
    }

    private Vector3 GetRandomPointOnSphere()
    {
        Vector3 randomDirection =
            Random.onUnitSphere;

        return station.position +
               randomDirection *
               spawnRadius;
    }
}