using UnityEngine;

public class PlayerStationRange : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform station;

    [Header("Spawn")]
    [SerializeField] private float spawnDistance = 60f;

    [Header("Range")]
    [SerializeField] private float maxDistance = 100f;

    private void Awake()
    {
        FindStation();
    }

    private void Start()
    {
        FindStation();

        if (station == null)
        {
            Debug.LogError("ERRO: estação não encontrada!");
            return;
        }

        Debug.Log("=== PLAYER SPAWN ===");
        Debug.Log("Estação: " + station.name);
        Debug.Log("Posição da estação: " + station.position);

        SpawnPlayer();

        Debug.Log("Posição final do Player: " + transform.position);

        Debug.Log(
            "Distância Player -> Estação: " +
            Vector3.Distance(transform.position, station.position)
        );
    }

    private void Update()
    {
        LimitDistance();
    }

    private void FindStation()
    {
        if (station != null)
            return;

        GameObject stationObject =
            GameObject.FindWithTag("Station");

        if (stationObject != null)
        {
            station =
                stationObject.transform;

            Debug.Log(
                "PlayerStationRange encontrou a estação automaticamente: " +
                station.name
            );
        }
        else
        {
            Debug.LogError(
                "PlayerStationRange: Nenhum objeto com a Tag 'Station' foi encontrado!"
            );
        }
    }

    private void SpawnPlayer()
    {
        if (station == null)
        {
            Debug.LogError(
                "PlayerStationRange: Station não foi definida!"
            );

            return;
        }

        // Escolhe uma direção aleatória ao redor da estação
        Vector3 direction =
            Random.onUnitSphere;

        // Mantém o spawn no plano horizontal
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector3.forward;
        }

        direction.Normalize();

        // Posiciona o Player
        transform.position =
            station.position +
            direction *
            spawnDistance;

        // Faz o Player olhar para a estação
        Vector3 lookDirection =
            station.position -
            transform.position;

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            transform.rotation =
                Quaternion.LookRotation(
                    lookDirection
                );
        }

        Debug.Log(
            "Player spawnado em: " +
            transform.position
        );

        Debug.Log(
            "Distância até a estação: " +
            Vector3.Distance(
                transform.position,
                station.position
            )
        );
    }

    private void LimitDistance()
    {
        if (station == null)
            return;

        Vector3 offset =
            transform.position -
            station.position;

        float distance =
            offset.magnitude;

        if (distance > maxDistance)
        {
            Vector3 direction =
                offset.normalized;

            transform.position =
                station.position +
                direction *
                maxDistance;
        }
    }
}