using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform station;
    [SerializeField] private Camera playerCamera;

    [Header("Radar UI")]
    [SerializeField] private RectTransform radarArea;

    [Header("Arrow Prefabs")]
    [SerializeField] private GameObject asteroidArrowPrefab;
    [SerializeField] private GameObject enemyArrowPrefab;
    [SerializeField] private GameObject powerUpArrowPrefab;

    [Header("Radar Range")]
    [SerializeField] private float radarRange = 250f;

    [Header("Arrow Position")]
    [SerializeField] private float edgeDistance = 15f;

    [Header("Colors")]
    [SerializeField] private Color farColor = Color.green;
    [SerializeField] private Color mediumColor = Color.yellow;
    [SerializeField] private Color closeColor = Color.red;
    [SerializeField] private Color enemyColor = Color.magenta;
    [SerializeField] private Color powerUpColor = Color.cyan;

    [Header("Asteroid Icon Sizes")]
    [SerializeField] private float largeAsteroidSize = 40f;
    [SerializeField] private float mediumAsteroidSize = 28f;
    [SerializeField] private float smallAsteroidSize = 18f;

    [Header("Station Distance")]
    [SerializeField] private float mediumDistance = 120f;
    [SerializeField] private float closeDistance = 60f;

    // Guarda qual seta pertence a cada objeto
    private Dictionary<GameObject, GameObject> arrows =
        new Dictionary<GameObject, GameObject>();


    private void Update()
    {
        UpdateRadar();
    }


    private void UpdateRadar()
    {
        if (player == null ||
            playerCamera == null ||
            radarArea == null)
            return;

        List<GameObject> currentTargets =
            new List<GameObject>();


        // =====================================================
        // ASTEROIDES
        // =====================================================

        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag("Asteroide");

        foreach (GameObject asteroid in asteroids)
        {
            if (IsInsideRadarRange(asteroid))
            {
                currentTargets.Add(asteroid);

                UpdateTarget(
                    asteroid,
                    asteroidArrowPrefab,
                    true,
                    false
                );
            }
        }


        // =====================================================
        // NAVES INIMIGAS
        // =====================================================

        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Inimigo");

        foreach (GameObject enemy in enemies)
        {
            if (IsInsideRadarRange(enemy))
            {
                currentTargets.Add(enemy);

                UpdateTarget(
                    enemy,
                    enemyArrowPrefab,
                    false,
                    false
                );
            }
        }


        // =====================================================
        // POWER-UPS
        // =====================================================

        GameObject[] powerUps =
            GameObject.FindGameObjectsWithTag("PowerUp");

        foreach (GameObject powerUp in powerUps)
        {
            if (IsInsideRadarRange(powerUp))
            {
                currentTargets.Add(powerUp);

                UpdateTarget(
                    powerUp,
                    powerUpArrowPrefab,
                    false,
                    true
                );
            }
        }


        // =====================================================
        // REMOVE SETAS INEXISTENTES
        // =====================================================

        List<GameObject> toRemove =
            new List<GameObject>();

        foreach (var pair in arrows)
        {
            if (!currentTargets.Contains(pair.Key))
            {
                if (pair.Value != null)
                {
                    Destroy(pair.Value);
                }

                toRemove.Add(pair.Key);
            }
        }

        foreach (GameObject target in toRemove)
        {
            arrows.Remove(target);
        }
    }


    // =====================================================
    // VERIFICA DISTÂNCIA
    // =====================================================

    private bool IsInsideRadarRange(GameObject target)
    {
        if (target == null)
            return false;

        float distance =
            Vector3.Distance(
                player.position,
                target.transform.position
            );

        return distance <= radarRange;
    }


    // =====================================================
    // CRIA / ATUALIZA A SETA
    // =====================================================

    private void UpdateTarget(
        GameObject target,
        GameObject arrowPrefab,
        bool isAsteroid,
        bool isPowerUp
    )
    {
        if (target == null ||
            arrowPrefab == null)
            return;


        GameObject arrowObject;


        // Cria a seta caso ainda não exista
        if (!arrows.ContainsKey(target))
        {
            arrowObject =
                Instantiate(
                    arrowPrefab,
                    radarArea
                );

            arrows.Add(
                target,
                arrowObject
            );
        }
        else
        {
            arrowObject =
                arrows[target];
        }


        if (arrowObject == null)
            return;


        RectTransform arrow =
            arrowObject.GetComponent<RectTransform>();


        if (arrow == null)
            return;


        // =====================================================
        // POSIÇÃO
        // =====================================================

        UpdateArrowPosition(
            target.transform,
            arrow
        );


        // =====================================================
        // ASTEROIDE
        // =====================================================

        if (isAsteroid)
        {
            UpdateAsteroidColor(
                target.transform,
                arrowObject
            );

            UpdateAsteroidSize(
                target,
                arrow
            );
        }


        // =====================================================
        // POWER-UP
        // =====================================================

        else if (isPowerUp)
        {
            Image image =
                arrowObject.GetComponent<Image>();

            if (image != null)
            {
                image.color =
                    powerUpColor;
            }

            // Tamanho padrão do Power-Up
            arrow.sizeDelta =
                new Vector2(
                    35f,
                    35f
                );
        }


        // =====================================================
        // INIMIGO
        // =====================================================

        else
        {
            Image image =
                arrowObject.GetComponent<Image>();

            if (image != null)
            {
                image.color =
                    enemyColor;
            }

            // Tamanho padrão para inimigos
            arrow.sizeDelta =
                new Vector2(
                    30f,
                    30f
                );
        }
    }


    // =====================================================
    // POSIÇÃO DA SETA NA BORDA DO RADAR
    // =====================================================

    private void UpdateArrowPosition(
        Transform target,
        RectTransform arrow
    )
    {
        Vector3 direction =
            target.position -
            player.position;


        // Converte a direção do mundo
        // para a direção relativa à câmera
        Vector3 localDirection =
            playerCamera.transform.InverseTransformDirection(
                direction
            );


        // Calcula o ângulo
        float angle =
            Mathf.Atan2(
                localDirection.x,
                localDirection.z
            ) *
            Mathf.Rad2Deg;


        // Calcula a posição da seta
        float radians =
            angle *
            Mathf.Deg2Rad;


        float radius =
            Mathf.Min(
                radarArea.rect.width,
                radarArea.rect.height
            ) / 2f -
            edgeDistance;


        Vector2 position =
            new Vector2(
                Mathf.Sin(radians),
                Mathf.Cos(radians)
            ) *
            radius;


        arrow.anchoredPosition =
            position;


        // Faz a seta apontar para o objeto
        arrow.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                -angle
            );
    }


    // =====================================================
    // COR DOS ASTEROIDES
    // =====================================================

    private void UpdateAsteroidColor(
        Transform asteroid,
        GameObject arrowObject
    )
    {
        if (station == null)
            return;


        Image image =
            arrowObject.GetComponent<Image>();


        if (image == null)
            return;


        float distance =
            Vector3.Distance(
                asteroid.position,
                station.position
            );


        // PERTO DA ESTAÇÃO
        if (distance <= closeDistance)
        {
            image.color =
                closeColor;
        }

        // DISTÂNCIA MÉDIA
        else if (distance <= mediumDistance)
        {
            image.color =
                mediumColor;
        }

        // LONGE DA ESTAÇÃO
        else
        {
            image.color =
                farColor;
        }
    }


    // =====================================================
    // TAMANHO DO ASTEROIDE
    // =====================================================

    private void UpdateAsteroidSize(
        GameObject asteroidObject,
        RectTransform arrow
    )
    {
        Asteroide asteroid =
            asteroidObject.GetComponentInParent<Asteroide>();


        if (asteroid == null)
            return;


        float size =
            mediumAsteroidSize;


        switch (asteroid.Size)
        {
            case Asteroide.AsteroidSize.Large:

                size =
                    largeAsteroidSize;

                break;


            case Asteroide.AsteroidSize.Medium:

                size =
                    mediumAsteroidSize;

                break;


            case Asteroide.AsteroidSize.Small:

                size =
                    smallAsteroidSize;

                break;
        }


        arrow.sizeDelta =
            new Vector2(
                size,
                size
            );
    }
}