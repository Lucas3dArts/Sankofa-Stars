using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [Header("Asteroid")]
    [SerializeField] private GameObject asteroidPrefab;

    [Header("Spawn")]
    [SerializeField] private int asteroidAmount = 5;

    [Header("Camera")]
    [SerializeField] private Camera asteroidsCamera;

    [Header("Spawn Margin")]
    [SerializeField] private float spawnMargin = 0.1f;

    [Header("Player Safety")]
    [SerializeField] private Transform player;
    [SerializeField] private float minimumDistanceFromPlayer = 2f;


    [Header("Waves")]
    [SerializeField] private int initialAsteroids = 5;
    [SerializeField] private int asteroidsIncreasePerWave = 2;
    [SerializeField] private float delayBetweenWaves = 2f;
    [SerializeField] private float maxWaveTime = 30f;

    private int currentWave = 0;

    

    private void Start()
    {
        if (asteroidsCamera == null)
        {
            GameObject cameraObject =
                GameObject.Find("AsteroidsCamera");

            if (cameraObject != null)
            {
                asteroidsCamera =
                    cameraObject.GetComponent<Camera>();
            }
        }

        if (asteroidsCamera == null)
        {
            Debug.LogError(
                "AsteroidsSpawner: AsteroidsCamera não encontrada!"
            );

            return;
        }

        StartCoroutine(WaveLoop());
    }

    private System.Collections.IEnumerator WaveLoop()
    {
        while (true)
        {
            currentWave++;

            int asteroidAmount =
                initialAsteroids +
                ((currentWave - 1) *
                asteroidsIncreasePerWave);

            Debug.Log(
                "=== WAVE " +
                currentWave +
                " ==="
            );

            Debug.Log(
                "Asteroides: " +
                asteroidAmount
            );

            for (int i = 0; i < asteroidAmount; i++)
            {
                SpawnAsteroid();
            }

            float timer = 0f;

            while (
                GameObject.FindGameObjectsWithTag("Asteroide").Length > 0 &&
                timer < maxWaveTime
            )
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (GameObject.FindGameObjectsWithTag("Asteroide").Length == 0)
            {
                Debug.Log(
                    "Wave " +
                    currentWave +
                    " concluída: todos os asteroides foram destruídos."
                );
            }
            else
            {
                Debug.Log(
                    "Tempo máximo da Wave " +
                    currentWave +
                    " atingido!"
                );
            }

            Debug.Log(
                "Próxima Wave em " +
                delayBetweenWaves +
                " segundos."
            );

            yield return new WaitForSeconds(
                delayBetweenWaves
            );
        }
    }

    private void SpawnAsteroids()
    {
        if (asteroidPrefab == null)
        {
            Debug.LogError(
                "AsteroidsSpawner: Asteroid Prefab não definido!"
            );

            return;
        }

        for (int i = 0; i < asteroidAmount; i++)
        {
            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        Vector3 spawnPosition;

        int attempts = 0;
        int maxAttempts = 20;

        do
        {
            Vector3 viewportPosition = new Vector3(
                Random.Range(spawnMargin, 1f - spawnMargin),
                Random.Range(spawnMargin, 1f - spawnMargin),
                0f
            );

            float distanceFromCamera =
                Mathf.Abs(
                    transform.position.z -
                    asteroidsCamera.transform.position.z
                );

            viewportPosition.z = distanceFromCamera;

            spawnPosition =
                asteroidsCamera.ViewportToWorldPoint(
                    viewportPosition
                );

            spawnPosition.z =
                transform.position.z;

            attempts++;

        } while (
            player != null &&
            Vector3.Distance(
                spawnPosition,
                player.position
            ) < minimumDistanceFromPlayer &&
            attempts < maxAttempts
        );

        GameObject asteroid =
            Instantiate(
                asteroidPrefab,
                spawnPosition,
                Quaternion.identity
            );

        asteroid.transform.SetParent(
            transform.parent
        );

        Debug.Log(
            "Asteroide criado em: " +
            asteroid.transform.position
        );
    }
}