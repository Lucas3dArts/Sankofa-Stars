using UnityEngine;

public class StationTurret : MonoBehaviour
{
    [Header("Turret")]
    [SerializeField] private Transform turretHead;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 100f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float projectileSpeed = 100f;

    [Header("Minigun")]
    [SerializeField] private Transform barrelPivot;
    [SerializeField] private float barrelSpinSpeed = 2000f;

    private Transform currentTarget;
    private float nextFireTime;

    private void Update()
    {
        FindTarget();

        if (currentTarget == null)
            return;
        if (currentTarget != null)
        {
            Debug.Log(
                "Alvo: " +
                currentTarget.name +
                " Pos: " +
                currentTarget.position);
        }
        RotateTowardsTarget();
        RotateBarrel();
        TryShoot();
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionRange);

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            Transform target = null;

            Asteroide asteroid = col.GetComponentInParent<Asteroide>();

            if (asteroid != null)
            {
                target = asteroid.transform;
            }

            if (target == null)
            {
                if (col.CompareTag("Inimigo") ||
                    col.transform.root.CompareTag("Inimigo"))
                {
                    target = col.transform.root;
                }
            }

            if (target == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                target.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        currentTarget = closestTarget;
    }

    private void RotateTowardsTarget()
    {
        if (turretHead == null || currentTarget == null)
            return;

        Vector3 direction =
            currentTarget.position - turretHead.position;

        turretHead.rotation =
            Quaternion.LookRotation(direction);
    }

    private void RotateBarrel()
    {
        if (barrelPivot == null)
            return;

        barrelPivot.Rotate(
           Vector3.forward,
           barrelSpinSpeed * Time.deltaTime,
           Space.Self);
    }

    private void TryShoot()
    {
        if (projectilePrefab == null)
            return;

        if (firePoint == null)
            return;

        if (currentTarget == null)
            return;

        if (Time.time < nextFireTime)
            return;

        float angle = Vector3.Angle(
            turretHead.forward,
            currentTarget.position - turretHead.position);

        // Só atira quando estiver mirando no alvo
        if (angle > 5f)
            return;

        nextFireTime = Time.time + fireRate;

        Vector3 direction =
            (currentTarget.position -
            firePoint.position).normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
                direction * projectileSpeed;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange);
    }
}