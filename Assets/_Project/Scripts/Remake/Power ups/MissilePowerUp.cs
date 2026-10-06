using UnityEngine;

public class MissilePowerUp : MonoBehaviour
{
    [Header("Missile Power")]
    [SerializeField] private float duration = 10f;
    [SerializeField] private float fireRate = 0.5f;

    [SerializeField] private GameObject missilePrefab;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            CollectPowerUp();
            return;
        }

        Projectile projectile =
            other.GetComponentInParent<Projectile>();

        if (projectile != null)
        {
            CollectPowerUp();

            Destroy(projectile.gameObject);
        }
    }

    private void CollectPowerUp()
    {
        if (collected)
            return;

        collected = true;

        MissileLauncher launcher =
            FindObjectOfType<MissileLauncher>();

        if (launcher != null)
        {
            launcher.StorePowerUp(
                duration,
                fireRate,
                missilePrefab
            );
        }
        else
        {
            Debug.LogError(
                "MissilePowerUp: " +
                "MissileLauncher não encontrado!"
            );
        }

        Debug.Log(
            "Power-up de mísseis coletado!"
        );

        Destroy(gameObject);
    }
}