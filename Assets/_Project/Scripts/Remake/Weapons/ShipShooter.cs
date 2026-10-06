using UnityEngine;
using UnityEngine.InputSystem;

public class ShipShooter : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 100f;

    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime;

    void Update()
    {
        HandleFireInput();
    }

    void HandleFireInput()
    {
        bool isFiring =
            Mouse.current.leftButton.isPressed ||
            Keyboard.current.spaceKey.isPressed;

        if (isFiring && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }
    }
}