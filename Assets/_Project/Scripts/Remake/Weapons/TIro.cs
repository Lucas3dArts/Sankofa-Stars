using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damage = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    void HandleHit(GameObject other)
    {
        Asteroide asteroid =
            other.GetComponentInParent<Asteroide>();

        if (asteroid != null)
        {
            asteroid.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        EnemyShip enemy =
            other.GetComponentInParent<EnemyShip>();

        if (enemy != null)
        {
            Debug.Log("Tiro atingiu nave inimiga!");

            enemy.TakeDamage(
                Mathf.RoundToInt(damage)
            );

            Destroy(gameObject);
            return;
        }

        EnemyShipStation stationEnemy =
            other.GetComponentInParent<EnemyShipStation>();

        if (stationEnemy != null)
        {
            Debug.Log(
                "Tiro atingiu nave inimiga que ataca a estação!"
            );

            stationEnemy.TakeDamage(
                Mathf.RoundToInt(damage)
            );

            Destroy(gameObject);
            return;
        }
    }
}