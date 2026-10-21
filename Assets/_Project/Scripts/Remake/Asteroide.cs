using System;
using UnityEngine;

public class Asteroide : MonoBehaviour
{
    public enum AsteroidSize
    {
        Large,
        Medium,
        Small
    }

    private Transform station;
    [SerializeField] private float destroyDistance = 250f;

    public event Action OnDestroyed;

    [Header("Asteroid Settings")]
    [SerializeField] private AsteroidSize size = AsteroidSize.Large;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Split Prefab")]
    [SerializeField] private GameObject asteroidPrefab;

    [Header("Damage")]
    [SerializeField] private int largeDamage = 3;
    [SerializeField] private int mediumDamage = 2;
    [SerializeField] private int smallDamage = 1;

    [Header("Station Damage")]
    [SerializeField] private int largeStationDamage = 100;
    [SerializeField] private int mediumStationDamage = 50;
    [SerializeField] private int smallStationDamage = 25;

    private void Start()
    {
        GameObject stationObject = GameObject.FindWithTag("Station");

        if (stationObject != null)
        {
            station = stationObject.transform;
        }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (station == null)
            return;

        float distance =
            Vector3.Distance(transform.position, station.position);

        if (distance > destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Split();

        HighScoreManager.Instance.AddScore(10);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosionSound();
        }


        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }

    void Split()
    {
        if (size == AsteroidSize.Small)
            return;

        int amount = (size == AsteroidSize.Large)
        ? UnityEngine.Random.Range(1, 2)
        : UnityEngine.Random.Range(2, 3);

        AsteroidSize nextSize = (size == AsteroidSize.Large)
            ? AsteroidSize.Medium
            : AsteroidSize.Small;

        for(int i = 0; i < amount; i++)
{
            Vector3 dir = UnityEngine.Random.onUnitSphere;
            Vector3 spawnPos = transform.position + dir * 4f;

            GameObject asteroid = Instantiate(
                asteroidPrefab,
                spawnPos,
                UnityEngine.Random.rotation
            );

            Asteroide newAsteroid = asteroid.GetComponent<Asteroide>();

            if (newAsteroid != null)
            {
                newAsteroid.SetSize(nextSize);
            }

            Rigidbody rb = asteroid.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(dir * UnityEngine.Random.Range(3f, 8f), ForceMode.Impulse);
                rb.AddTorque(UnityEngine.Random.onUnitSphere * 50f, ForceMode.Impulse);
            }
        }
    }

    public AsteroidSize Size
    {
        get { return size; }
    }

    public void SetSize(AsteroidSize newSize)
    {
        size = newSize;

        switch (size)
        {
            case AsteroidSize.Large:
                maxHealth = 100f;
                transform.localScale = Vector3.one;
                break;

            case AsteroidSize.Medium:
                maxHealth = 50f;
                transform.localScale = Vector3.one * 2f;
                break;

            case AsteroidSize.Small:
                maxHealth = 25f;
                transform.localScale = Vector3.one * 1.5f;
                break;
        }

        currentHealth = maxHealth;
    }

    public int Damage
    {
        get
        {
            switch (size)
            {
                case AsteroidSize.Large:
                    return largeDamage;

                case AsteroidSize.Medium:
                    return mediumDamage;

                case AsteroidSize.Small:
                    return smallDamage;

                default:
                    return 1;
            }
        }
    }

    public int StationDamage
    {
        get
        {
            switch (size)
            {
                case AsteroidSize.Large:
                    return largeStationDamage;

                case AsteroidSize.Medium:
                    return mediumStationDamage;

                case AsteroidSize.Small:
                    return smallStationDamage;

                default:
                    return 25;
            }
        }
    }

    

    int GetScore()
    {
        return size switch
        {
            AsteroidSize.Large => 100,
            AsteroidSize.Medium => 50,
            AsteroidSize.Small => 25,
            _ => 0
        };
    }

}
