using UnityEngine;

public class RadarPowerUp : MonoBehaviour
{
    public enum PowerType
    {
        Freeze,
        Lightning,
        Shield,
        Missile
    }

    [Header("Radar")]
    [SerializeField] private PowerType powerType;

    public PowerType Type => powerType;
}