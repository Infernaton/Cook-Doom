using UnityEngine;
using Entity;
using CustomAttribute;

/**
 * Each modifier will add or remove stats for the projectile
 * in percentage with the default value init in the player (projectile part) gameobject
 */
[CreateAssetMenu(fileName = "New Modifier", menuName = "Player/Projectile/New Modifier")]
public class ProjectileModifier : Modifier
{
    #region Stat attribute
    public float MovingSpeed = 0;
    public float Damage = 0;
    public float Piercing = 0;
    #endregion

    #region Customise attribute
    public float Size = 0;
    [UnDisplayable] public Color? NewColor; //Depending on the assets found, might be useless
    #endregion

    public float ExplosionRadius = 0; //Add later on

    public bool ResetingProjectileModifier = false;
}