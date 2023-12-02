using UnityEngine;
using Entity;
using CustomAttribute;

/**
 * Each modifier will add or remove stats for the player
 * in percentage with the default value init in the player gameobject
 */
[CreateAssetMenu(fileName = "New Modifier", menuName = "Player/New Modifier")]
public class PlayerModifier : Modifier
{
    public float MovementSpeed = 0;
    public float ProtectionRadius = 0;
    public float MaxHealth = 0;
    public float FireRate = 0;

    [Range(1, 5), UnDisplayable]
    public int NumberProjectile = 1;

    public bool ResetingPlayerModifier = false;
}
