using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float rotationSpeed = 5f;

    [Tooltip("Distância na qual a nave para de avançar e começa a atacar.")]
    [SerializeField] private float attackDistance = 30f;

    [Tooltip("Impede que a nave encoste completamente no jogador.")]
    [SerializeField] private float stoppingDistance = 10f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float projectileSpeed = 50f;

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
                "EnemyShip precisa de um Rigidbody no mesmo objeto.",
                gameObject
            );
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        FollowPlayer();
    }

    private void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distanceToPlayer <= attackDistance)
        {
            Shoot();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "Nenhum objeto com a tag Player foi encontrado.",
                gameObject
            );
        }
    }

    private void FollowPlayer()
    {
        Vector3 directionToPlayer =
            player.position - rb.position;

        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= 0.01f)
            return;

        directionToPlayer.Normalize();

        RotateTowardsPlayer(directionToPlayer);

        // Continua seguindo até alcançar a distância mínima.
        if (distanceToPlayer > stoppingDistance)
        {
            Vector3 nextPosition =
                rb.position +
                directionToPlayer *
                moveSpeed *
                Time.fixedDeltaTime;

            rb.MovePosition(nextPosition);
        }
    }

    private void RotateTowardsPlayer(Vector3 directionToPlayer)
    {
        if (directionToPlayer == Vector3.zero)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                directionToPlayer,
                Vector3.up
            );

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
            Vector3 shootingDirection =
                (player.position - firePoint.position).normalized;

            projectileRb.linearVelocity =
                shootingDirection * projectileSpeed;
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosionSound();
        }

        if (ExplosionEffectManager.Instance != null)
        {
            ExplosionEffectManager.Instance.CreateExplosion(
                transform.position,
                1.5f
            );
        }



        Destroy(gameObject);
    }
}