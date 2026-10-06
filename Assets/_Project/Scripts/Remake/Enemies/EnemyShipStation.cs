using UnityEngine;

public class EnemyShipStation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform station;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 50f;
    [SerializeField] private float fireRate = 1.5f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 50f;

    [Header("Damage")]
    [SerializeField] private int stationDamage = 25;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private float nextFireTime;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        if (rb == null)
        {
            Debug.LogError(
                "EnemyShipStation: A nave não possui Rigidbody!"
            );
        }
    }

    private void Start()
    {
        // Procura a estação automaticamente
        if (station == null)
        {
            GameObject stationObject =
                GameObject.FindWithTag("Station");

            if (stationObject != null)
            {
                station = stationObject.transform;

                Debug.Log(
                    "EnemyShipStation encontrou a estação: " +
                    station.name
                );
            }
            else
            {
                Debug.LogError(
                    "EnemyShipStation: Não encontrou nenhum objeto com a Tag 'Station'!"
                );
            }
        }
    }

    private void FixedUpdate()
    {
        if (station == null)
            return;

        float distance =
            Vector3.Distance(
                rb.position,
                station.position
            );

        // Ainda está longe da estação
        if (distance > attackDistance)
        {
            ChaseStation();
        }
        else
        {
            FaceStation();
        }
    }

    private void Update()
    {
        if (station == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                station.position
            );

        if (distance <= attackDistance)
        {
            Shoot();
        }
    }

    private void ChaseStation()
    {
        // Direção exata até a estação
        Vector3 direction =
            (station.position - rb.position).normalized;

        // Rotaciona a nave para a estação
        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        rb.MoveRotation(newRotation);

        // Move diretamente na direção da estação
        Vector3 newPosition =
            rb.position +
            direction *
            moveSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void FaceStation()
    {
        Vector3 direction =
            station.position - transform.position;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed *
                Time.fixedDeltaTime
            );

        rb.MoveRotation(newRotation);
    }

    private void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                "EnemyShipStation: Projectile Prefab não definido!"
            );

            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning(
                "EnemyShipStation: FirePoint não definido!"
            );

            return;
        }

        nextFireTime =
            Time.time + fireRate;

        GameObject projectile =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                firePoint.rotation
            );

        Rigidbody projectileRb =
            projectile.GetComponent<Rigidbody>();

        if (projectileRb != null)
        {
            projectileRb.linearVelocity =
                firePoint.forward *
                projectileSpeed;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(
            "Nave de ataque à estação recebeu dano. Vida: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(
            "Nave de ataque à estação destruída!"
        );

        if (HighScoreManager.Instance != null)
        {
            HighScoreManager.Instance.AddScore(150);
        }

        Destroy(gameObject);
    }
}