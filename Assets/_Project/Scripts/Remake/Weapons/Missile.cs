using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 50f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 15f;
    [SerializeField] private float damage = 100f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 8f;

    [Header("Target Distance")]
    [SerializeField] private float hitDistance = 2f;

    private Transform target;
    private bool exploded = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (exploded)
            return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Verifica a distância até o alvo
        float distance =
            Vector3.Distance(
                transform.position,
                target.position
            );

        if (distance <= hitDistance)
        {
            Explode();
            return;
        }

        // Direção do alvo
        Vector3 direction =
            target.position - transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }

        // Movimento
        transform.position +=
            transform.forward *
            speed *
            Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (exploded)
            return;

        Asteroide asteroid =
            other.GetComponentInParent<Asteroide>();

        EnemyShip enemy =
            other.GetComponentInParent<EnemyShip>();

        EnemyShipStation stationEnemy =
            other.GetComponentInParent<EnemyShipStation>();

        if (
            asteroid != null ||
            enemy != null ||
            stationEnemy != null
        )
        {
            Debug.Log(
                "Míssil entrou em contato com um alvo!"
            );

            Explode();
        }
    }

    private void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log(
            "💥 MÍSSIL EXPLODIU!"
        );

        Collider[] objects =
            Physics.OverlapSphere(
                transform.position,
                explosionRadius
            );

        HashSet<GameObject> damagedObjects =
            new HashSet<GameObject>();

        foreach (Collider collider in objects)
        {
            // =========================
            // ASTEROIDE
            // =========================

            Asteroide asteroid =
                collider.GetComponentInParent<Asteroide>();

            if (asteroid != null)
            {
                if (!damagedObjects.Contains(asteroid.gameObject))
                {
                    damagedObjects.Add(asteroid.gameObject);

                    Debug.Log(
                        "💥 Explosão atingiu ASTEROIDE: " +
                        asteroid.name
                    );

                    asteroid.TakeDamage(damage);
                }
            }

            // =========================
            // NAVE INIMIGA
            // =========================

            EnemyShip enemy =
                collider.GetComponentInParent<EnemyShip>();

            if (enemy != null)
            {
                if (!damagedObjects.Contains(enemy.gameObject))
                {
                    damagedObjects.Add(enemy.gameObject);

                    Debug.Log(
                        "💥 Explosão atingiu NAVE!"
                    );

                    enemy.TakeDamage(
                        Mathf.RoundToInt(damage)
                    );
                }
            }

            // =========================
            // NAVE DA ESTAÇÃO
            // =========================

            EnemyShipStation stationEnemy =
                collider.GetComponentInParent<EnemyShipStation>();

            if (stationEnemy != null)
            {
                if (!damagedObjects.Contains(
                    stationEnemy.gameObject))
                {
                    damagedObjects.Add(
                        stationEnemy.gameObject
                    );

                    Debug.Log(
                        "💥 Explosão atingiu NAVE DA ESTAÇÃO!"
                    );

                    stationEnemy.TakeDamage(
                        Mathf.RoundToInt(damage)
                    );
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}