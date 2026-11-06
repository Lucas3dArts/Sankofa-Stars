using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform station;

    [Tooltip("Use a câmera principal da nave, não a HUD_Camera.")]
    [SerializeField] private Camera playerCamera;

    [Header("Radar UI")]
    [Tooltip("Área do GlassHUD em que o radar poderá aparecer.")]
    [SerializeField] private RectTransform radarArea;

    [Header("Asteroid Icon")]
    [SerializeField] private GameObject asteroidArrowPrefab;

    [Header("Enemy Icons")]
    [Tooltip("Ícone do inimigo relacionado à estação.")]
    [SerializeField] private GameObject stationEnemyArrowPrefab;

    [Tooltip("Ícone da nave inimiga que persegue o jogador.")]
    [SerializeField] private GameObject playerEnemyArrowPrefab;

    [Header("Power-Up Icons")]
    [SerializeField] private GameObject FreezeArrowPrefab;
    [SerializeField] private GameObject LightningArrowPrefab;
    [SerializeField] private GameObject ShieldArrowPrefab;
    [SerializeField] private GameObject MissileArrowPrefab;

    [Header("Radar Range")]
    [Tooltip("Distância máxima para um objeto aparecer no radar.")]
    [SerializeField] private float radarRange = 500f;

    [Header("Circular Radar Area")]
    [Tooltip("Margem entre os ícones e o limite do Radar Area.")]
    [SerializeField] private float circularEdgePadding = 30f;

    [Tooltip("Controla o tamanho horizontal da área circular.")]
    [Range(0.1f, 1f)]
    [SerializeField] private float horizontalRadiusMultiplier = 0.85f;

    [Tooltip("Controla o tamanho vertical da área circular.")]
    [Range(0.1f, 1f)]
    [SerializeField] private float verticalRadiusMultiplier = 0.85f;

    [Tooltip(
        "Espaço inferior reservado para vida, poderes, pontuação e outras informações."
    )]
    [SerializeField] private float reservedBottomSpace = 140f;

    [Header("Center Detection")]
    [Tooltip(
        "Quando o alvo estiver dentro deste ângulo frontal, o ícone desaparecerá."
    )]
    [Range(1f, 30f)]
    [SerializeField] private float centerHideAngle = 8f;

    [Tooltip(
        "O ícone reaparecerá quando o alvo ultrapassar este ângulo."
    )]
    [Range(1f, 40f)]
    [SerializeField] private float centerShowAngle = 11f;

    [Tooltip("Velocidade da transição de aparecer e desaparecer.")]
    [SerializeField] private float iconFadeSpeed = 8f;

    [Header("Icon Movement")]
    [Tooltip("Velocidade com que os ícones se movimentam no radar.")]
    [SerializeField] private float iconMovementSpeed = 12f;

    [Tooltip("Deixa os alvos atrás da nave mais transparentes.")]
    [SerializeField] private bool fadeTargetsBehind = true;

    [Range(0f, 1f)]
    [SerializeField] private float behindTargetAlpha = 0.45f;

    [Header("Distance Colors")]
    [SerializeField] private Color farColor = Color.green;
    [SerializeField] private Color mediumColor = Color.yellow;
    [SerializeField] private Color closeColor = Color.red;

    [Header("Power-Up Fixed Colors")]
    [SerializeField] private Color freezeColor = Color.cyan;
    [SerializeField] private Color lightningColor = Color.blue;
    [SerializeField] private Color shieldColor = Color.magenta;
    [SerializeField] private Color missileColor = Color.white;

    [Header("Distance Thresholds")]
    [Tooltip("Abaixo desta distância, o ícone fica vermelho.")]
    [SerializeField] private float closeDistance = 60f;

    [Tooltip("Abaixo desta distância, o ícone fica amarelo.")]
    [SerializeField] private float mediumDistance = 120f;

    [Header("Icon Sizes")]
    [SerializeField] private float stationEnemySize = 35f;
    [SerializeField] private float playerEnemySize = 30f;
    [SerializeField] private float powerUpSize = 35f;

    [Header("Asteroid Icon Sizes")]
    [SerializeField] private float largeAsteroidSize = 40f;
    [SerializeField] private float mediumAsteroidSize = 28f;
    [SerializeField] private float smallAsteroidSize = 18f;

    private readonly Dictionary<GameObject, GameObject> arrows =
        new Dictionary<GameObject, GameObject>();

    /*
     * Guarda os ícones ocultados por estarem
     * na frente da câmera.
     *
     * A diferença entre centerHideAngle e
     * centerShowAngle impede que o ícone fique piscando.
     */
    private readonly HashSet<GameObject> centeredHiddenArrows =
        new HashSet<GameObject>();

    private void Awake()
    {
        ValidateValues();
    }

    private void OnValidate()
    {
        ValidateValues();
    }

    private void ValidateValues()
    {
        if (centerShowAngle <= centerHideAngle)
        {
            centerShowAngle = centerHideAngle + 1f;
        }

        radarRange = Mathf.Max(0f, radarRange);

        circularEdgePadding =
            Mathf.Max(0f, circularEdgePadding);

        reservedBottomSpace =
            Mathf.Max(0f, reservedBottomSpace);

        iconFadeSpeed =
            Mathf.Max(0f, iconFadeSpeed);

        iconMovementSpeed =
            Mathf.Max(0f, iconMovementSpeed);

        closeDistance =
            Mathf.Max(0f, closeDistance);

        mediumDistance =
            Mathf.Max(closeDistance, mediumDistance);

        horizontalRadiusMultiplier = Mathf.Clamp(
            horizontalRadiusMultiplier,
            0.1f,
            1f
        );

        verticalRadiusMultiplier = Mathf.Clamp(
            verticalRadiusMultiplier,
            0.1f,
            1f
        );
    }

    private void Update()
    {
        UpdateRadar();
    }

    private void UpdateRadar()
    {
        if (player == null ||
            playerCamera == null ||
            radarArea == null)
        {
            return;
        }

        HashSet<GameObject> currentTargets =
            new HashSet<GameObject>();

        UpdateAsteroids(currentTargets);
        UpdateEnemies(currentTargets);
        UpdatePowerUps(currentTargets);

        RemoveMissingArrows(currentTargets);
    }

    // =====================================================
    // ASTEROIDES
    // =====================================================

    private void UpdateAsteroids(
        HashSet<GameObject> currentTargets
    )
    {
        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag("Asteroide");

        foreach (GameObject asteroidObject in asteroids)
        {
            if (asteroidObject == null)
                continue;

            if (!IsInsideRadarRange(asteroidObject))
                continue;

            currentTargets.Add(asteroidObject);

            GameObject arrowObject = GetOrCreateArrow(
                asteroidObject,
                asteroidArrowPrefab
            );

            if (arrowObject == null)
                continue;

            UpdateCommonArrow(
                asteroidObject.transform,
                arrowObject
            );

            /*
             * A cor do asteroide é calculada
             * pela distância até a estação.
             */
            SetDistanceColor(
                arrowObject,
                asteroidObject.transform,
                station
            );

            UpdateAsteroidSize(
                asteroidObject,
                arrowObject
            );
        }
    }

    // =====================================================
    // INIMIGOS
    // =====================================================

    private void UpdateEnemies(
        HashSet<GameObject> currentTargets
    )
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Inimigo");

        foreach (GameObject enemyObject in enemies)
        {
            if (enemyObject == null)
                continue;

            EnemyShipStation stationEnemy =
                enemyObject.GetComponentInParent<EnemyShipStation>();

            EnemyShip playerEnemy =
                enemyObject.GetComponentInParent<EnemyShip>();

            if (stationEnemy != null)
            {
                GameObject enemyRoot =
                    stationEnemy.gameObject;

                if (!IsInsideRadarRange(enemyRoot))
                    continue;

                /*
                 * Evita processar o mesmo inimigo mais de uma vez
                 * caso existam filhos com a tag Inimigo.
                 */
                if (!currentTargets.Add(enemyRoot))
                    continue;

                UpdateStationEnemy(enemyRoot);
            }
            else if (playerEnemy != null)
            {
                GameObject enemyRoot =
                    playerEnemy.gameObject;

                if (!IsInsideRadarRange(enemyRoot))
                    continue;

                if (!currentTargets.Add(enemyRoot))
                    continue;

                UpdatePlayerEnemy(enemyRoot);
            }
        }
    }

    private void UpdateStationEnemy(
        GameObject enemyObject
    )
    {
        GameObject arrowObject = GetOrCreateArrow(
            enemyObject,
            stationEnemyArrowPrefab
        );

        if (arrowObject == null)
            return;

        UpdateCommonArrow(
            enemyObject.transform,
            arrowObject
        );

        /*
         * O inimigo da estação muda de cor
         * conforme a distância até a estação.
         */
        SetDistanceColor(
            arrowObject,
            enemyObject.transform,
            station
        );

        SetArrowSize(
            arrowObject,
            stationEnemySize
        );
    }

    private void UpdatePlayerEnemy(
        GameObject enemyObject
    )
    {
        GameObject arrowObject = GetOrCreateArrow(
            enemyObject,
            playerEnemyArrowPrefab
        );

        if (arrowObject == null)
            return;

        UpdateCommonArrow(
            enemyObject.transform,
            arrowObject
        );

        /*
         * A nave inimiga comum muda de cor
         * conforme a distância até o jogador.
         */
        SetDistanceColor(
            arrowObject,
            enemyObject.transform,
            player
        );

        SetArrowSize(
            arrowObject,
            playerEnemySize
        );
    }

    // =====================================================
    // POWER-UPS
    // =====================================================

    private void UpdatePowerUps(
        HashSet<GameObject> currentTargets
    )
    {
        GameObject[] powerUps =
            GameObject.FindGameObjectsWithTag("PowerUp");

        foreach (GameObject powerUpObject in powerUps)
        {
            if (powerUpObject == null)
                continue;

            RadarPowerUp radarPowerUp =
                powerUpObject.GetComponentInParent<RadarPowerUp>();

            if (radarPowerUp == null)
            {
                Debug.LogWarning(
                    "O Power-Up " +
                    powerUpObject.name +
                    " não possui o componente RadarPowerUp.",
                    powerUpObject
                );

                continue;
            }

            GameObject powerRoot =
                radarPowerUp.gameObject;

            if (!IsInsideRadarRange(powerRoot))
                continue;

            if (!currentTargets.Add(powerRoot))
                continue;

            GameObject arrowPrefab =
                GetPowerUpArrowPrefab(radarPowerUp.Type);

            Color fixedColor =
                GetPowerUpColor(radarPowerUp.Type);

            GameObject arrowObject = GetOrCreateArrow(
                powerRoot,
                arrowPrefab
            );

            if (arrowObject == null)
                continue;

            UpdateCommonArrow(
                powerRoot.transform,
                arrowObject
            );

            SetArrowColor(
                arrowObject,
                fixedColor
            );

            SetArrowSize(
                arrowObject,
                powerUpSize
            );
        }
    }

    private GameObject GetPowerUpArrowPrefab(
        RadarPowerUp.PowerType powerType
    )
    {
        switch (powerType)
        {
            case RadarPowerUp.PowerType.Freeze:
                return FreezeArrowPrefab;

            case RadarPowerUp.PowerType.Lightning:
                return LightningArrowPrefab;

            case RadarPowerUp.PowerType.Shield:
                return ShieldArrowPrefab;

            case RadarPowerUp.PowerType.Missile:
                return MissileArrowPrefab;

            default:
                return FreezeArrowPrefab;
        }
    }

    private Color GetPowerUpColor(
        RadarPowerUp.PowerType powerType
    )
    {
        switch (powerType)
        {
            case RadarPowerUp.PowerType.Freeze:
                return freezeColor;

            case RadarPowerUp.PowerType.Lightning:
                return lightningColor;

            case RadarPowerUp.PowerType.Shield:
                return shieldColor;

            case RadarPowerUp.PowerType.Missile:
                return missileColor;

            default:
                return Color.white;
        }
    }

    // =====================================================
    // CRIAÇÃO DOS ÍCONES
    // =====================================================

    private GameObject GetOrCreateArrow(
        GameObject target,
        GameObject arrowPrefab
    )
    {
        if (target == null || arrowPrefab == null)
            return null;

        if (arrows.TryGetValue(
            target,
            out GameObject existingArrow))
        {
            if (existingArrow != null)
            {
                return existingArrow;
            }

            arrows.Remove(target);
        }

        GameObject newArrow = Instantiate(
            arrowPrefab,
            radarArea
        );

        newArrow.name =
            arrowPrefab.name +
            "_" +
            target.name;

        RectTransform arrowRect =
            newArrow.GetComponent<RectTransform>();

        if (arrowRect != null)
        {
            arrowRect.anchorMin =
                new Vector2(0.5f, 0.5f);

            arrowRect.anchorMax =
                new Vector2(0.5f, 0.5f);

            arrowRect.pivot =
                new Vector2(0.5f, 0.5f);

            arrowRect.anchoredPosition =
                GetRadarCenter();

            arrowRect.localScale =
                Vector3.one;
        }

        CanvasGroup canvasGroup =
            GetOrCreateCanvasGroup(newArrow);

        canvasGroup.alpha = 1f;

        arrows.Add(
            target,
            newArrow
        );

        return newArrow;
    }

    private void UpdateCommonArrow(
        Transform target,
        GameObject arrowObject
    )
    {
        if (target == null || arrowObject == null)
            return;

        RectTransform arrow =
            arrowObject.GetComponent<RectTransform>();

        if (arrow == null)
            return;

        CanvasGroup canvasGroup =
            GetOrCreateCanvasGroup(arrowObject);

        UpdateArrowPosition(
            target,
            arrow,
            canvasGroup
        );
    }

    // =====================================================
    // POSICIONAMENTO CIRCULAR
    // =====================================================

    private void UpdateArrowPosition(
        Transform target,
        RectTransform arrow,
        CanvasGroup canvasGroup
    )
    {
        if (target == null ||
            arrow == null ||
            canvasGroup == null ||
            playerCamera == null ||
            radarArea == null)
        {
            return;
        }

        Vector3 directionToTarget =
            target.position -
            playerCamera.transform.position;

        if (directionToTarget.sqrMagnitude <= 0.001f)
            return;

        Vector3 normalizedDirection =
            directionToTarget.normalized;

        /*
         * O ângulo é calculado usando a câmera principal
         * da nave, e não a câmera da Render Texture.
         */
        float angleFromCenter = Vector3.Angle(
            playerCamera.transform.forward,
            normalizedDirection
        );

        Vector3 localDirection =
            playerCamera.transform.InverseTransformDirection(
                normalizedDirection
            );

        bool targetIsBehind =
            localDirection.z <= 0f;

        bool shouldHide = ShouldHideCenteredIcon(
            arrow.gameObject,
            angleFromCenter,
            targetIsBehind
        );

        Vector2 targetPosition =
            CalculateCircularRadarPosition(
                localDirection
            );

        float movementFactor =
            1f -
            Mathf.Exp(
                -iconMovementSpeed *
                Time.unscaledDeltaTime
            );

        arrow.anchoredPosition = Vector2.Lerp(
            arrow.anchoredPosition,
            targetPosition,
            movementFactor
        );

        UpdateCircularArrowRotation(
            arrow,
            targetPosition
        );

        UpdateIconVisibility(
            canvasGroup,
            shouldHide,
            targetIsBehind
        );
    }

    private Vector2 CalculateCircularRadarPosition(
        Vector3 localDirection
    )
    {
        Rect radarRect =
            radarArea.rect;

        float safeReservedBottomSpace =
            Mathf.Clamp(
                reservedBottomSpace,
                0f,
                radarRect.height * 0.8f
            );

        float availableHeight =
            radarRect.height -
            safeReservedBottomSpace;

        availableHeight =
            Mathf.Max(availableHeight, 1f);

        /*
         * O raio considera o tamanho do próprio ícone,
         * evitando que metade dele fique fora da área.
         */
        float radiusX =
            radarRect.width * 0.5f -
            circularEdgePadding;

        float radiusY =
            availableHeight * 0.5f -
            circularEdgePadding;

        radiusX =
            Mathf.Max(
                0f,
                radiusX *
                horizontalRadiusMultiplier
            );

        radiusY =
            Mathf.Max(
                0f,
                radiusY *
                verticalRadiusMultiplier
            );

        Vector2 radarCenter =
            GetRadarCenter();

        /*
         * X representa esquerda e direita.
         * Z representa frente e trás.
         *
         * Frente aparece na parte superior do radar.
         * Atrás aparece na parte inferior.
         */
        Vector2 circularDirection = new Vector2(
            localDirection.x,
            localDirection.z
        );

        if (circularDirection.sqrMagnitude <= 0.001f)
        {
            circularDirection = Vector2.up;
        }
        else
        {
            circularDirection.Normalize();
        }

        return radarCenter + new Vector2(
            circularDirection.x * radiusX,
            circularDirection.y * radiusY
        );
    }

    private Vector2 GetRadarCenter()
    {
        if (radarArea == null)
            return Vector2.zero;

        Rect radarRect =
            radarArea.rect;

        float safeReservedBottomSpace =
            Mathf.Clamp(
                reservedBottomSpace,
                0f,
                radarRect.height * 0.8f
            );

        float availableHeight =
            radarRect.height -
            safeReservedBottomSpace;

        /*
         * Desloca o centro para cima para impedir que
         * o radar cubra vida, pontuação e poderes.
         */
        return new Vector2(
            radarRect.center.x,
            radarRect.yMin +
            safeReservedBottomSpace +
            availableHeight * 0.5f
        );
    }

    private void UpdateCircularArrowRotation(
        RectTransform arrow,
        Vector2 targetPosition
    )
    {
        if (arrow == null || radarArea == null)
            return;

        Vector2 radarCenter =
            GetRadarCenter();

        Vector2 directionFromCenter =
            targetPosition -
            radarCenter;

        if (directionFromCenter.sqrMagnitude <= 0.001f)
        {
            arrow.localRotation =
                Quaternion.identity;

            return;
        }

        float angle =
            Mathf.Atan2(
                directionFromCenter.y,
                directionFromCenter.x
            ) *
            Mathf.Rad2Deg;

        /*
         * Este cálculo considera que o desenho da seta
         * aponta originalmente para cima.
         */
        arrow.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle - 90f
            );
    }

    // =====================================================
    // VISIBILIDADE NO CENTRO
    // =====================================================

    private bool ShouldHideCenteredIcon(
        GameObject arrowObject,
        float angleFromCenter,
        bool targetIsBehind
    )
    {
        if (arrowObject == null)
            return false;

        /*
         * Alvos atrás da câmera não devem desaparecer
         * pela regra de centralização.
         */
        if (targetIsBehind)
        {
            centeredHiddenArrows.Remove(arrowObject);
            return false;
        }

        bool isHidden =
            centeredHiddenArrows.Contains(arrowObject);

        if (!isHidden &&
            angleFromCenter <= centerHideAngle)
        {
            centeredHiddenArrows.Add(arrowObject);
            return true;
        }

        if (isHidden &&
            angleFromCenter >= centerShowAngle)
        {
            centeredHiddenArrows.Remove(arrowObject);
            return false;
        }

        return isHidden;
    }

    private void UpdateIconVisibility(
        CanvasGroup canvasGroup,
        bool shouldHide,
        bool targetIsBehind
    )
    {
        if (canvasGroup == null)
            return;

        float targetAlpha;

        if (shouldHide)
        {
            targetAlpha = 0f;
        }
        else if (fadeTargetsBehind &&
                 targetIsBehind)
        {
            targetAlpha = behindTargetAlpha;
        }
        else
        {
            targetAlpha = 1f;
        }

        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            targetAlpha,
            iconFadeSpeed *
            Time.unscaledDeltaTime
        );
    }

    private CanvasGroup GetOrCreateCanvasGroup(
        GameObject arrowObject
    )
    {
        CanvasGroup canvasGroup =
            arrowObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                arrowObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.ignoreParentGroups = false;

        return canvasGroup;
    }

    // =====================================================
    // CORES
    // =====================================================

    private void SetDistanceColor(
        GameObject arrowObject,
        Transform target,
        Transform distanceReference
    )
    {
        if (arrowObject == null ||
            target == null ||
            distanceReference == null)
        {
            return;
        }

        float distance = Vector3.Distance(
            target.position,
            distanceReference.position
        );

        Color selectedColor;

        if (distance <= closeDistance)
        {
            selectedColor = closeColor;
        }
        else if (distance <= mediumDistance)
        {
            selectedColor = mediumColor;
        }
        else
        {
            selectedColor = farColor;
        }

        SetArrowColor(
            arrowObject,
            selectedColor
        );
    }

    private void SetArrowColor(
        GameObject arrowObject,
        Color color
    )
    {
        if (arrowObject == null)
            return;

        Image image =
            arrowObject.GetComponent<Image>();

        if (image == null)
        {
            image =
                arrowObject.GetComponentInChildren<Image>();
        }

        if (image != null)
        {
            image.color = color;
        }
    }

    // =====================================================
    // TAMANHOS
    // =====================================================

    private void SetArrowSize(
        GameObject arrowObject,
        float size
    )
    {
        if (arrowObject == null)
            return;

        RectTransform arrow =
            arrowObject.GetComponent<RectTransform>();

        if (arrow != null)
        {
            arrow.sizeDelta =
                new Vector2(size, size);
        }
    }

    private void UpdateAsteroidSize(
        GameObject asteroidObject,
        GameObject arrowObject
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
                size = largeAsteroidSize;
                break;

            case Asteroide.AsteroidSize.Medium:
                size = mediumAsteroidSize;
                break;

            case Asteroide.AsteroidSize.Small:
                size = smallAsteroidSize;
                break;
        }

        SetArrowSize(
            arrowObject,
            size
        );
    }

    // =====================================================
    // ALCANCE
    // =====================================================

    private bool IsInsideRadarRange(
        GameObject target
    )
    {
        if (target == null || player == null)
            return false;

        float distance = Vector3.Distance(
            player.position,
            target.transform.position
        );

        return distance <= radarRange;
    }

    // =====================================================
    // REMOÇÃO DOS ÍCONES
    // =====================================================

    private void RemoveMissingArrows(
        HashSet<GameObject> currentTargets
    )
    {
        List<GameObject> targetsToRemove =
            new List<GameObject>();

        List<GameObject> arrowsToRemove =
            new List<GameObject>();

        foreach (
            KeyValuePair<GameObject, GameObject> pair
            in arrows)
        {
            bool targetWasDestroyed =
                pair.Key == null;

            bool targetLeftRadar =
                !targetWasDestroyed &&
                !currentTargets.Contains(pair.Key);

            bool arrowWasDestroyed =
                pair.Value == null;

            if (!targetWasDestroyed &&
                !targetLeftRadar &&
                !arrowWasDestroyed)
            {
                continue;
            }

            if (pair.Value != null)
            {
                arrowsToRemove.Add(pair.Value);
                Destroy(pair.Value);
            }

            targetsToRemove.Add(pair.Key);
        }

        foreach (GameObject arrow in arrowsToRemove)
        {
            centeredHiddenArrows.Remove(arrow);
        }

        foreach (GameObject target in targetsToRemove)
        {
            arrows.Remove(target);
        }
    }

    private void OnDisable()
    {
        foreach (
            KeyValuePair<GameObject, GameObject> pair
            in arrows)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value);
            }
        }

        arrows.Clear();
        centeredHiddenArrows.Clear();
    }
}