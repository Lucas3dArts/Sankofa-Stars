using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePowerUp : MonoBehaviour
{
    [Header("Freeze Settings")]
    [SerializeField] private float freezeDuration = 5f;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        // Player encostou no poder
        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            ActivateFreeze();
            return;
        }

        // Tiro do Player atingiu o poder
        Projectile projectile =
            other.GetComponentInParent<Projectile>();

        if (projectile != null)
        {
            ActivateFreeze();

            Destroy(projectile.gameObject);
        }
    }

    private void ActivateFreeze()
    {
        if (activated)
            return;

        activated = true;

        Debug.Log(
            "CONGELAMENTO ATIVADO por " +
            freezeDuration +
            " segundos!"
        );

        // Mostra o nome e a duração na HUD
        if (PowerUpDurationUI.Instance != null)
        {
            PowerUpDurationUI.Instance.StartPowerUp(
                "Fecha Caminho de Exu",
                freezeDuration
            );
        }

        StartCoroutine(FreezeAllEnemies());

        Destroy(gameObject);
    }

    private IEnumerator FreezeAllEnemies()
    {
        List<Rigidbody> frozenObjects =
            new List<Rigidbody>();

        // =========================
        // CONGELA ASTEROIDES
        // =========================

        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag("Asteroide");

        foreach (GameObject asteroid in asteroids)
        {
            Rigidbody rb =
                asteroid.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.isKinematic = true;

                frozenObjects.Add(rb);
            }
        }

        // =========================
        // CONGELA NAVES
        // =========================

        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Inimigo");

        foreach (GameObject enemy in enemies)
        {
            Rigidbody rb =
                enemy.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.isKinematic = true;

                frozenObjects.Add(rb);
            }
        }

        Debug.Log(
            "Todos os asteroides e naves foram congelados!"
        );

        // Aguarda o tempo do poder
        yield return new WaitForSeconds(
            freezeDuration
        );

        // =========================
        // DESCONGELA
        // =========================

        foreach (Rigidbody rb in frozenObjects)
        {
            if (rb == null)
                continue;

            rb.isKinematic = false;
        }

        Debug.Log(
            "Congelamento terminou!"
        );
    }
}