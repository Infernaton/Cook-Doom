using UnityEngine;
using Entity;

/**
 * Each modifier will add or remove stats for the player
 * in percentage with the default value init in the player gameobject
 */
[CreateAssetMenu(fileName = "New Modifier", menuName = "Player/New Modifier")]
public class PlayerModifier : Modifier
{
    public float MovementSpeed;
    public float ProtectionRadius;
    public float MaxHealth;
    public float FireRate;

    [Range(1, 5)]
    public int NumberProjectile = 1;

    public bool ResetingPlayerModifier = false;
}
