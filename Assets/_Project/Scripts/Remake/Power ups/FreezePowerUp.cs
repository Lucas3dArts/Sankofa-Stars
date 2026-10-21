using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePowerUp : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private float freezeDuration = 5f;

    [Header("Visual")]
    [SerializeField] private GameObject tridentePrefab;
    [Header("BG Effect")]
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    private bool activated = false;

    private class FrozenObject
    {
        public Rigidbody rb;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (other.CompareTag("Player") || other.CompareTag("Tiro"))
        {
            ActivateFreeze();

            if (other.CompareTag("Tiro"))
                Destroy(other.gameObject);

            // Faz o power-up sumir
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
                mr.enabled = false;
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

        if (PowerUpDurationUI.Instance != null)
        {
            PowerUpDurationUI.Instance.StartPowerUp(
                "Fecha Caminho de Exu",
                freezeDuration
            );
        }

        backgroundMaterial.SetColor("_Color_1", color1);
        backgroundMaterial.SetColor("_Color_2", color2);
        backgroundMaterial.SetFloat("_Power_Enable", 1);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerUpSound();
        }


        StartCoroutine(FreezeAllEnemies());
    }

    private IEnumerator FreezeAllEnemies()
    {
        List<FrozenObject> frozenObjects = new List<FrozenObject>();
        List<GameObject> tridents = new List<GameObject>();

        // ASTEROIDES
        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag("Asteroide");

        foreach (GameObject asteroid in asteroids)
        {
            Rigidbody rb = asteroid.GetComponent<Rigidbody>();

            if (rb != null)
            {
                frozenObjects.Add(new FrozenObject
                {
                    rb = rb,
                    velocity = rb.linearVelocity,
                    angularVelocity = rb.angularVelocity
                });

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;

                if (tridentePrefab != null)
                {
                    GameObject trident = Instantiate(
                        tridentePrefab,
                        asteroid.transform.position,
                        asteroid.transform.rotation,
                        asteroid.transform
                    );

                    tridents.Add(trident);
                }
            }
        }

        // INIMIGOS
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Inimigo");

        foreach (GameObject enemy in enemies)
        {
            Rigidbody rb = enemy.GetComponent<Rigidbody>();

            if (rb != null)
            {
                frozenObjects.Add(new FrozenObject
                {
                    rb = rb,
                    velocity = rb.linearVelocity,
                    angularVelocity = rb.angularVelocity
                });

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;

                if (tridentePrefab != null)
                {
                    GameObject trident = Instantiate(
                        tridentePrefab,
                        enemy.transform.position,
                        enemy.transform.rotation,
                        enemy.transform
                    );

                    tridents.Add(trident);
                }
            }
        }

        Debug.Log("Todos os inimigos e asteroides foram congelados.");

        yield return new WaitForSeconds(freezeDuration);

        // DESCONGELA
        foreach (FrozenObject obj in frozenObjects)
        {
            if (obj.rb != null)
            {
                obj.rb.isKinematic = false;
                obj.rb.linearVelocity = obj.velocity;
                obj.rb.angularVelocity = obj.angularVelocity;
            }
        }

        // REMOVE TRIDENTES
        foreach (GameObject trident in tridents)
        {
            if (trident != null)
            {
                Destroy(trident);
            }
        }

        backgroundMaterial.SetFloat("_Power_Enable", 0);

        Debug.Log("Congelamento encerrado.");

        Destroy(gameObject);
    }
}
