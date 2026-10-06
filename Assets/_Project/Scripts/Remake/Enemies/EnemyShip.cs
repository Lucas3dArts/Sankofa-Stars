using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float attackDistance = 30f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private float nextFireTime;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            ChasePlayer();
        }
        else
        {
            FacePlayer();
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            Shoot();
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction =
            (player.position - transform.position).normalized;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        rb.MoveRotation(newRotation);

        rb.MovePosition(
            rb.position +
            transform.forward * moveSpeed * Time.fixedDeltaTime
        );
    }

    private void FacePlayer()
    {
        Vector3 direction =
            (player.position - transform.position).normalized;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        rb.MoveRotation(newRotation);
    }

    private void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (projectilePrefab == null || firePoint == null)
            return;

        nextFireTime = Time.time + fireRate;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody projectileRb =
            projectile.GetComponent<Rigidbody>();

        if (projectileRb != null)
        {
            projectileRb.linearVelocity =
                firePoint.forward * 50f;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(
            "Nave inimiga recebeu dano. Vida: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Nave inimiga destruída!");

        if (HighScoreManager.Instance != null)
        {
            HighScoreManager.Instance.AddScore(100);
        }

        Destroy(gameObject);
    }
}