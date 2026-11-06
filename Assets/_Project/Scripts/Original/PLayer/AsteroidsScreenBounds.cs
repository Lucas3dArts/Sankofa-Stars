using UnityEngine;

public class AsteroidsScreenBounds : MonoBehaviour
{
    [Header("Câmera que renderiza o Asteroids")]
    [SerializeField] private Camera asteroidsCamera;

    [Header("Área permitida da tela")]
    [Range(0f, 0.49f)]
    [SerializeField] private float horizontalMargin = 0.20f;

    [Range(0f, 0.49f)]
    [SerializeField] private float verticalMargin = 0.20f;

    [Header("Configuração")]
    [SerializeField] private bool keepOriginalZ = true;

    private void LateUpdate()
    {
        if (asteroidsCamera == null)
            return;

        Vector3 viewportPosition =
            asteroidsCamera.WorldToViewportPoint(transform.position);

        // Mantém o objeto entre 20% e 80% da tela, por exemplo.
        viewportPosition.x = Mathf.Clamp(
            viewportPosition.x,
            horizontalMargin,
            1f - horizontalMargin
        );

        viewportPosition.y = Mathf.Clamp(
            viewportPosition.y,
            verticalMargin,
            1f - verticalMargin
        );

        Vector3 correctedPosition =
            asteroidsCamera.ViewportToWorldPoint(viewportPosition);

        if (keepOriginalZ)
            correctedPosition.z = transform.position.z;

        transform.position = correctedPosition;
    }
}