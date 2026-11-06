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
        TryActivatePowerUp(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryActivatePowerUp(collision.gameObject);
    }

    private void TryActivatePowerUp(GameObject objectThatTouched)
    {
        if (activated || objectThatTouched == null)
            return;

        GameObject detectedObject = FindPlayerOrShot(objectThatTouched);

        if (detectedObject == null)
            return;

        bool isPlayer = detectedObject.CompareTag("Player");
        bool isShot = detectedObject.CompareTag("Tiro");

        if (!isPlayer && !isShot)
            return;

        Debug.Log(
            "Freeze Power-Up ativado por: " +
            detectedObject.name +
            " | Tag: " +
            detectedObject.tag
        );

        ActivateFreeze();

        if (isShot)
        {
            Destroy(detectedObject);
        }

        HidePowerUp();
    }

    private GameObject FindPlayerOrShot(GameObject touchedObject)
    {
        Transform currentTransform = touchedObject.transform;

        while (currentTransform != null)
        {
            if (currentTransform.CompareTag("Player") ||
                currentTransform.CompareTag("Tiro"))
            {
                return currentTransform.gameObject;
            }

            currentTransform = currentTransform.parent;
        }

        Rigidbody attachedRigidbody =
            touchedObject.GetComponentInParent<Rigidbody>();

        if (attachedRigidbody != null)
        {
            GameObject rigidbodyObject = attachedRigidbody.gameObject;

            if (rigidbodyObject.CompareTag("Player") ||
                rigidbodyObject.CompareTag("Tiro"))
            {
                return rigidbodyObject;
            }
        }

        return null;
    }

    private void HidePowerUp()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider currentCollider in colliders)
        {
            currentCollider.enabled = false;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer currentRenderer in renderers)
        {
            currentRenderer.enabled = false;
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

        if (BackgroundPowerEffectManager.Instance != null)
        {
            BackgroundPowerEffectManager.Instance.ActivateEffect(
                this,
                color1,
                color2
            );
        }

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
            FreezeObject(asteroid, frozenObjects, tridents);
        }

        // INIMIGOS
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Inimigo");

        foreach (GameObject enemy in enemies)
        {
            FreezeObject(enemy, frozenObjects, tridents);
        }

        Debug.Log(
            "Todos os inimigos e asteroides foram congelados."
        );

        yield return new WaitForSeconds(freezeDuration);

        // DESCONGELA
        foreach (FrozenObject frozenObject in frozenObjects)
        {
            if (frozenObject.rb != null)
            {
                frozenObject.rb.isKinematic = false;
                frozenObject.rb.linearVelocity =
                    frozenObject.velocity;

                frozenObject.rb.angularVelocity =
                    frozenObject.angularVelocity;
            }
        }

        // REMOVE OS TRIDENTES
        foreach (GameObject trident in tridents)
        {
            if (trident != null)
            {
                Destroy(trident);
            }
        }

        if (BackgroundPowerEffectManager.Instance != null)
        {
            BackgroundPowerEffectManager.Instance.DeactivateEffect(this);
        }

        Debug.Log("Congelamento encerrado.");

        Destroy(gameObject);
    }

    private void FreezeObject(
        GameObject objectToFreeze,
        List<FrozenObject> frozenObjects,
        List<GameObject> tridents
    )
    {
        if (objectToFreeze == null)
            return;

        Rigidbody rb =
            objectToFreeze.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = objectToFreeze.GetComponentInChildren<Rigidbody>();
        }

        if (rb == null)
            return;

        frozenObjects.Add(
            new FrozenObject
            {
                rb = rb,
                velocity = rb.linearVelocity,
                angularVelocity = rb.angularVelocity
            }
        );

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (tridentePrefab != null)
        {
            GameObject trident = Instantiate(
                tridentePrefab,
                objectToFreeze.transform.position,
                objectToFreeze.transform.rotation,
                objectToFreeze.transform
            );

            tridents.Add(trident);
        }
    }
}