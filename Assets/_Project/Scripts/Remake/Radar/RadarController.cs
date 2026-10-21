using System.Collections.Generic;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    [Header("Referências")]
    public Transform station;

    [Header("Radar")]
    public float radarRange = 1000f;
    public float radarRadius = 0.4f;

    [Header("Atualização")]
    public float updateInterval = 0.5f;

    [Header("Rescan")]
    public float rescanInterval = 2f;

    [Header("Prefabs")]
    public RadarMarker enemyPrefab;
    public RadarMarker asteroidPrefab;
    public RadarMarker powerupPrefab;
    public RadarMarker friendlyPrefab;

    private float updateTimer;
    private float rescanTimer;

    private readonly Dictionary<RadarTarget, RadarMarker>
        trackedTargets =
        new Dictionary<RadarTarget, RadarMarker>();

    void Start()
    {
        RescanTargets();
        UpdateMarkers();
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        rescanTimer += Time.deltaTime;

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateMarkers();
        }

        if (rescanTimer >= rescanInterval)
        {
            rescanTimer = 0f;
            RescanTargets();
        }
    }

    void RescanTargets()
    {
        RadarTarget[] allTargets =
            FindObjectsByType<RadarTarget>(
                FindObjectsSortMode.None);

        HashSet<RadarTarget> currentTargets =
            new HashSet<RadarTarget>(allTargets);

        // Adiciona novos alvos
        foreach (RadarTarget target in allTargets)
        {
            if (target == null)
                continue;

            if (trackedTargets.ContainsKey(target))
                continue;

            RadarMarker prefab =
                GetPrefab(target.targetType);

            if (prefab == null)
                continue;

            RadarMarker marker =
                Instantiate(prefab, transform);

            marker.target = target.transform;
            marker.radar = this;

            trackedTargets.Add(target, marker);
        }

        // Remove alvos destruídos
        List<RadarTarget> toRemove =
            new List<RadarTarget>();

        foreach (var pair in trackedTargets)
        {
            RadarTarget target = pair.Key;
            RadarMarker marker = pair.Value;

            if (target == null)
            {
                if (marker != null)
                    Destroy(marker.gameObject);

                toRemove.Add(target);
            }
        }

        foreach (RadarTarget target in toRemove)
        {
            trackedTargets.Remove(target);
        }
    }

    void UpdateMarkers()
    {
        List<RadarTarget> toRemove =
            new List<RadarTarget>();

        foreach (var pair in trackedTargets)
        {
            RadarTarget target = pair.Key;
            RadarMarker marker = pair.Value;

            if (target == null ||
                marker == null ||
                marker.target == null)
            {
                if (marker != null)
                    Destroy(marker.gameObject);

                toRemove.Add(target);
                continue;
            }

            Vector3 relativePosition =
                station.InverseTransformPoint(
                    marker.target.position);

            float distance =
                relativePosition.magnitude;

            if (distance > radarRange)
            {
                marker.gameObject.SetActive(false);
                continue;
            }

            marker.gameObject.SetActive(true);

            Vector3 radarPosition =
                relativePosition /
                radarRange *
                radarRadius;

            marker.desiredPosition =
                radarPosition;
        }

        foreach (RadarTarget target in toRemove)
        {
            trackedTargets.Remove(target);
        }
    }

    RadarMarker GetPrefab(
        RadarTargetType type)
    {
        switch (type)
        {
            case RadarTargetType.Enemy:
                return enemyPrefab;

            case RadarTargetType.Asteroid:
                return asteroidPrefab;

            case RadarTargetType.Powerup:
                return powerupPrefab;

            case RadarTargetType.Friendly:
                return friendlyPrefab;

            default:
                return null;
        }
    }
}