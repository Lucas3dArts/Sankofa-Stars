using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ShipShooter : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 100f;

    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 0.2f;
    private float nextFireTime;

    [Header("Muzzle Flash")]
    [SerializeField] private VisualEffect muzzleFlash;

    private void Update()
    {
        HandleFireInput();
    }

    private void HandleFireInput()
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

    private void Shoot()
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

        PlayMuzzleFlash();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShootSound();
        }
    }

    private void PlayMuzzleFlash()
    {
        if (muzzleFlash == null)
            return;

        muzzleFlash.Stop();
        muzzleFlash.Reinit();
        muzzleFlash.Play();
    }
}