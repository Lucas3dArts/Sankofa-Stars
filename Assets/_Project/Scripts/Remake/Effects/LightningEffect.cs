using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.15f;

    private Vector3 originPosition;
    private Transform target;

    public void SetTarget(
        Vector3 newOriginPosition,
        Transform newTarget
    )
    {
        originPosition = newOriginPosition;
        target = newTarget;

        Destroy(
            gameObject,
            duration
        );
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction =
            target.position - originPosition;

        float distance =
            direction.magnitude;

        if (distance <= 0.01f)
            return;

        // Coloca o raio entre o ponto inicial e o alvo
        transform.position =
            (originPosition + target.position) / 2f;

        transform.rotation =
            Quaternion.LookRotation(direction);

        transform.localScale =
            new Vector3(
                1f,
                1f,
                distance
            );
    }
}