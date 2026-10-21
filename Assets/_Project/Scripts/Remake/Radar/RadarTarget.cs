using UnityEngine;

public enum RadarTargetType
{
    Asteroid,
    Enemy,
    Powerup,
    Friendly
}

public class RadarTarget : MonoBehaviour
{
    public RadarTargetType targetType;
}