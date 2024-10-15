using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AttackSpec")]
public class AttackSpec : ScriptableObject
{
    [field: SerializeField] public float DamageMultiplier { get; private set; }
    [field: SerializeField] public float CastAngleSize { get; private set; }
    [field: SerializeField] public Vector2 CastCenterPosition { get; private set; }
    [field: SerializeField] public Vector2 Direction { get; private set; }
    [field: SerializeField] public float Distance { get; private set; }
}
