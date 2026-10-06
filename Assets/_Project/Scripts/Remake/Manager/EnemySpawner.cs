using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject playerEnemyPrefab;
    [SerializeField] private GameObject stationEnemyPrefab;
    [SerializeField] private Transform station;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 150f;

    [Header("Player Enemy")]
    [SerializeField] private int initialPlayerEnemies = 1;
    [SerializeField] private int playerEnemiesIncreasePerWave = 1;

    [Header("Station Enemy")]
    [SerializeField] private int initialStationEnemies = 1;
    [SerializeField] private int stationEnemiesIncreasePerWave = 1;

    [Header("Wave Settings")]
    [SerializeField] private float maxWaveTime = 30f;
    [SerializeField] private float delayBetweenWaves = 2f;

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

            int playerEnemies =
                initialPlayerEnemies +
                ((currentWave - 1) *
                playerEnemiesIncreasePerWave);

            int stationEnemies =
                initialStationEnemies +
                ((currentWave - 1) *
                stationEnemiesIncreasePerWave);

            Debug.Log(
                "=== WAVE " +
                currentWave +
                " ==="
            );

            Debug.Log(
                "Inimigos Player: " +
                playerEnemies
            );

            Debug.Log(
                "Inimigos Estação: " +
                stationEnemies
            );

            // Spawn das naves que atacam o Player
            SpawnPlayerEnemies(playerEnemies);

            // Spawn das naves que atacam a Estação
            SpawnStationEnemies(stationEnemies);

            float timer = 0f;

            // Espera até todos os inimigos morrerem
            // ou até atingir o tempo máximo
            while (
                GameObject.FindGameObjectsWithTag("Inimigo").Length > 0
                && timer < maxWaveTime
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


    // teste do bug 

    private void Awake()
    {
        Time.timeScale = 1f;

        Debug.Log("=================================");
        Debug.Log("ENEMY SPAWNER FOI INICIALIZADO!");
        Debug.Log("Player Enemy Prefab: " + playerEnemyPrefab);
        Debug.Log("Station Enemy Prefab: " + stationEnemyPrefab);
        Debug.Log("Station: " + station);
        Debug.Log("=================================");
    }



    // =========================================
    // PLAYER ENEMIES
    // =========================================

    private void SpawnPlayerEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy(playerEnemyPrefab);
        }
    }

    // =========================================
    // STATION ENEMIES
    // =========================================

    private void SpawnStationEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy(stationEnemyPrefab);
        }
    }

    // =========================================
    // SPAWN
    // =========================================

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(
                "EnemySpawner: Prefab de inimigo não definido!"
            );

            return;
        }

        if (station == null)
        {
            Debug.LogError(
                "EnemySpawner: Station não foi definida!"
            );

            return;
        }

        // Direção completamente aleatória em 3D
        Vector3 direction = Random.onUnitSphere;

        // Distância do spawn em relação à estação
        Vector3 spawnPosition =
            station.position +
            direction * spawnDistance;

        GameObject enemy =
            Instantiate(
                enemyPrefab,
                spawnPosition,
                Random.rotation
            );

        Debug.Log(
            "Inimigo spawnado em: " +
            spawnPosition
        );
    }
}