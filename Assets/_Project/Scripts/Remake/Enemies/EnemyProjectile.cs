using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private int playerDamage = 1;
    [SerializeField] private int stationDamage = 25;
    [SerializeField] private float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // =========================
        // ATINGIU O PLAYER
        // =========================

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            Debug.Log("Player atingido pelo tiro inimigo!");

            playerHealth.TakeDamage(playerDamage);

            Destroy(gameObject);

            return;
        }

        // =========================
        // ATINGIU A ESTAÇÃO
        // =========================

        StationHealth stationHealth =
            other.GetComponentInParent<StationHealth>();

        if (stationHealth != null)
        {
            Debug.Log("Estação atingida pelo tiro inimigo!");

            stationHealth.TakeDamage(stationDamage);

            Destroy(gameObject);

            return;
        }
    }
}