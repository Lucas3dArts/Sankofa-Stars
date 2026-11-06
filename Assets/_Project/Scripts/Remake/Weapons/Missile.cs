using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Missile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 50f;

    [Tooltip("Velocidade de rotação em graus por segundo.")]
    [SerializeField] private float rotationSpeed = 360f;

    [Tooltip("Tempo usado para prever a posição futura do alvo.")]
    [SerializeField] private float predictionTime = 0.15f;

    [Tooltip("Limita a distância máxima da previsão.")]
    [SerializeField] private float maximumPredictionDistance = 15f;

    [Header("Launch")]
    [Tooltip("Tempo em que o míssil avança antes de começar a perseguir.")]
    [SerializeField] private float launchDelay = 0.08f;

    [Tooltip("Distância inicial sem colisão para evitar atingir a nave que disparou.")]
    [SerializeField] private float safeLaunchDistance = 2f;

    [Header("Collision Detection")]
    [Tooltip("Raio usado para verificar o trajeto do míssil.")]
    [SerializeField] private float collisionCheckRadius = 0.35f;

    [SerializeField] private LayerMask collisionLayers = ~0;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 15f;
    [SerializeField] private float damage = 100f;

    [Header("Explosion Visual")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float explosionEffectLifetime = 3f;
    [SerializeField] private Vector3 explosionEffectScale = Vector3.one;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 8f;

    [Header("Target Distance")]
    [SerializeField] private float hitDistance = 2f;

    private Transform target;
    private Rigidbody targetRigidbody;
    private Rigidbody rb;

    private Transform missileOwner;

    private bool exploded;
    private float launchTime;
    private Vector3 launchPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        Collider missileCollider = GetComponent<Collider>();

        if (missileCollider != null)
        {
            missileCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        launchTime = Time.time;
        launchPosition = rb.position;

        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            targetRigidbody =
                target.GetComponentInParent<Rigidbody>();

            Debug.Log(
                "Míssil seguindo o alvo: " +
                target.name
            );
        }
    }

    public void SetOwner(Transform newOwner)
    {
        missileOwner = newOwner;
    }

    private void FixedUpdate()
    {
        if (exploded)
            return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = GetPredictedTargetPosition();

        Vector3 directionToTarget =
            targetPosition - rb.position;

        float distanceToTarget =
            Vector3.Distance(
                rb.position,
                target.position
            );

        if (distanceToTarget <= hitDistance)
        {
            Explode();
            return;
        }

        bool canFollowTarget =
            Time.time >= launchTime + launchDelay;

        if (canFollowTarget &&
            directionToTarget.sqrMagnitude > 0.001f)
        {
            RotateTowardsTarget(directionToTarget.normalized);
        }

        MoveMissile();
    }

    private Vector3 GetPredictedTargetPosition()
    {
        Vector3 currentTargetPosition = target.position;

        if (targetRigidbody == null)
            return currentTargetPosition;

        Vector3 targetVelocity =
            targetRigidbody.linearVelocity;

        Vector3 predictedMovement =
            targetVelocity * predictionTime;

        predictedMovement = Vector3.ClampMagnitude(
            predictedMovement,
            maximumPredictionDistance
        );

        return currentTargetPosition + predictedMovement;
    }

    private void RotateTowardsTarget(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion desiredRotation =
            Quaternion.LookRotation(
                direction,
                Vector3.up
            );

        Quaternion nextRotation =
            Quaternion.RotateTowards(
                rb.rotation,
                desiredRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        rb.MoveRotation(nextRotation);
    }

    private void MoveMissile()
    {
        Vector3 movement =
            transform.forward *
            speed *
            Time.fixedDeltaTime;

        float movementDistance = movement.magnitude;

        if (movementDistance <= 0f)
            return;

        Vector3 movementDirection =
            movement.normalized;

        bool outsideSafeLaunchDistance =
            Vector3.Distance(
                launchPosition,
                rb.position
            ) >= safeLaunchDistance;

        if (outsideSafeLaunchDistance)
        {
            CheckMovementPath(
                movementDirection,
                movementDistance
            );

            if (exploded)
                return;
        }

        rb.MovePosition(rb.position + movement);
    }

    private void CheckMovementPath(
        Vector3 movementDirection,
        float movementDistance
    )
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            rb.position,
            collisionCheckRadius,
            movementDirection,
            movementDistance,
            collisionLayers,
            QueryTriggerInteraction.Collide
        );

        float nearestDistance = float.MaxValue;
        Collider nearestValidCollider = null;

        foreach (RaycastHit hit in hits)
        {
            Collider hitCollider = hit.collider;

            if (hitCollider == null)
                continue;

            if (hitCollider.transform.IsChildOf(transform))
                continue;

            if (missileOwner != null &&
                hitCollider.transform.IsChildOf(missileOwner))
            {
                continue;
            }

            if (!IsDamageableTarget(hitCollider))
                continue;

            if (hit.distance < nearestDistance)
            {
                nearestDistance = hit.distance;
                nearestValidCollider = hitCollider;
            }
        }

        if (nearestValidCollider != null)
        {
            rb.position =
                rb.position +
                movementDirection *
                nearestDistance;

            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (exploded || other == null)
            return;

        if (other.transform.IsChildOf(transform))
            return;

        if (missileOwner != null &&
            other.transform.IsChildOf(missileOwner))
        {
            return;
        }

        float distanceFromLaunch =
            Vector3.Distance(
                launchPosition,
                transform.position
            );

        if (distanceFromLaunch < safeLaunchDistance)
            return;

        if (IsDamageableTarget(other))
        {
            Debug.Log(
                "Míssil entrou em contato com um alvo!"
            );

            Explode();
        }
    }

    private bool IsDamageableTarget(Collider other)
    {
        if (other == null)
            return false;

        Asteroide asteroid =
            other.GetComponentInParent<Asteroide>();

        EnemyShip enemy =
            other.GetComponentInParent<EnemyShip>();

        EnemyShipStation stationEnemy =
            other.GetComponentInParent<EnemyShipStation>();

        return asteroid != null ||
               enemy != null ||
               stationEnemy != null;
    }

   

    private void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Collider missileCollider = GetComponent<Collider>();

        if (missileCollider != null)
        {
            missileCollider.enabled = false;
        }

        Debug.Log("💥 MÍSSIL EXPLODIU!");

        if (ExplosionEffectManager.Instance != null)
        {
            ExplosionEffectManager.Instance.CreateExplosion(
                transform.position
            );
        }

        Collider[] objects = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            collisionLayers,
            QueryTriggerInteraction.Collide
        );

        HashSet<GameObject> damagedObjects =
            new HashSet<GameObject>();

        foreach (Collider currentCollider in objects)
        {
            DamageAsteroid(
                currentCollider,
                damagedObjects
            );

            DamageEnemyShip(
                currentCollider,
                damagedObjects
            );

            DamageStationEnemy(
                currentCollider,
                damagedObjects
            );
        }

        Destroy(gameObject);
    }

    private void DamageAsteroid(
        Collider currentCollider,
        HashSet<GameObject> damagedObjects
    )
    {
        Asteroide asteroid =
            currentCollider.GetComponentInParent<Asteroide>();

        if (asteroid == null)
            return;

        if (!damagedObjects.Add(asteroid.gameObject))
            return;

        Debug.Log(
            "💥 Explosão atingiu ASTEROIDE: " +
            asteroid.name
        );

        asteroid.TakeDamage(damage);
    }

    private void DamageEnemyShip(
        Collider currentCollider,
        HashSet<GameObject> damagedObjects
    )
    {
        EnemyShip enemy =
            currentCollider.GetComponentInParent<EnemyShip>();

        if (enemy == null)
            return;

        if (!damagedObjects.Add(enemy.gameObject))
            return;

        Debug.Log("💥 Explosão atingiu NAVE!");

        enemy.TakeDamage(
            Mathf.RoundToInt(damage)
        );
    }

    private void DamageStationEnemy(
        Collider currentCollider,
        HashSet<GameObject> damagedObjects
    )
    {
        EnemyShipStation stationEnemy =
            currentCollider.GetComponentInParent<EnemyShipStation>();

        if (stationEnemy == null)
            return;

        if (!damagedObjects.Add(stationEnemy.gameObject))
            return;

        Debug.Log(
            "💥 Explosão atingiu NAVE DA ESTAÇÃO!"
        );

        stationEnemy.TakeDamage(
            Mathf.RoundToInt(damage)
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            collisionCheckRadius
        );
    }
}