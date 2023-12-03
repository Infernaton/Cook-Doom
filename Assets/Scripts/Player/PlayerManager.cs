using UnityEngine;
using UnityEngine.InputSystem;
using Entity;
using Utils;
using System.Collections.Generic;

public class PlayerController : LifeFormManager
{
    private Vector2 _movement;
    private Rigidbody _rigidBody;
    private float _lastSpawn;
    private GameManager gm;

    private bool _isShooting = false;

    [SerializeField] float m_MoveSpeed;
    [SerializeField] ProjectileManager m_Projectile;
    [SerializeField] Transform m_SpawnProjectile;
    [SerializeField] float m_ProtectionRadius;

    [SerializeField] float m_FireRate;

    [SerializeField] float m_ProjSpeed;
    [SerializeField] float m_ProjDamage;
    [SerializeField] int m_ProjPiercing;

    public PlayerModifier PlayerModifierMerge { get; private set; }
    [SerializeField] List<PlayerModifier> m_PlayerModifierList;
    public ProjectileModifier ProjModifierMerge { get; private set; }
    [SerializeField] List<ProjectileModifier> m_ProjectileModifierList;

    #region get
    public float GetProtectionRadius() => DMath.AddPercentage(m_ProtectionRadius, PlayerModifierMerge.ProtectionRadius);

    public int GetTotalItem() => m_PlayerModifierList.Count + m_ProjectileModifierList.Count;
    #endregion

    public void AddModifier(Modifier mod)
    {
        if (mod is PlayerModifier playerMod)
        {
            //When upgrading the player maxhealth we heal a portion of its current HP
            //Current issue -> MaxHealth is a percentage but InstantHeal(float recover) take an amount of HP the player gain
            if (playerMod.MaxHealth > 0)
            {
                float addedHealth = DMath.Percentage(m_HealthBase, playerMod.MaxHealth);
                _currentMaxHealth += addedHealth;
                InstantHeal(addedHealth);
            }

            m_PlayerModifierList.Add(playerMod);
            PlayerModifierMerge = MergeAllPlayerModifier();
        } 
        else if (mod is ProjectileModifier projMod)
        {
            m_ProjectileModifierList.Add(projMod);
            ProjModifierMerge = MergeAllProjModifier();
        }
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        ProjModifierMerge = MergeAllProjModifier();
        PlayerModifierMerge = MergeAllPlayerModifier();
    }

    private void Start()
    {
        _startLifeForm();
        gm = GameManager.Instance;
    }

    private void FixedUpdate()
    {
        float moveSpeed = DMath.AddPercentage(m_MoveSpeed, PlayerModifierMerge.MovementSpeed);
        _rigidBody.velocity = new Vector3(moveSpeed * _movement.x, _rigidBody.velocity.y, moveSpeed * _movement.y);
    }

    void Update()
    {
        if (_isShooting && (Time.time - _lastSpawn >= 1f / DMath.AddPercentage(m_FireRate, PlayerModifierMerge.FireRate))) SpawnProjectile();
        _updateLifeForm();
        UpdateLookAt();
    }

    private void SpawnProjectile()
    {
        for (int i = 0; i < PlayerModifierMerge.NumberProjectile; i++)
        {
            //Set the projectile all arround the player when getting a bonus
            float degree = (360 / PlayerModifierMerge.NumberProjectile) * i;
            ProjectileManager proj = Instantiate(m_Projectile, m_SpawnProjectile.position, Quaternion.AngleAxis(degree, transform.up) * transform.rotation);
            proj.transform.SetParent(transform.parent, true);

            proj.Speed = DMath.AddPercentage(m_ProjSpeed, ProjModifierMerge.MovingSpeed);
            proj.Damage = DMath.AddPercentage(m_ProjDamage, ProjModifierMerge.Damage);
            proj.Piercing = DMath.AddPercentage(m_ProjPiercing, ProjModifierMerge.Piercing);

            proj.transform.localScale += ProjModifierMerge.Size * proj.transform.localScale / 100;
            if (ProjModifierMerge.NewColor != null) proj.GetComponent<Material>().color = (Color)ProjModifierMerge.NewColor;
        }
        _lastSpawn = Time.time;
    }

    private void UpdateLookAt()
    {
        Vector2 mouse = Mouse.current.position.value;
        Vector3 startPos = new (mouse.x, mouse.y, Camera.main.nearClipPlane);

        //Get ray from mouse postion
        Ray rayCast = Camera.main.ScreenPointToRay(startPos);

        //Raycast and check if any object is hit
        Physics.Raycast(rayCast, out RaycastHit hit, Camera.main.farClipPlane);
        Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

        transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
    }

    #region Modifiers
    private ProjectileModifier MergeAllProjModifier()
    {
        ProjectileModifier mod = ScriptableObject.CreateInstance<ProjectileModifier>();
        mod.Rarity = 4;
        foreach (ProjectileModifier item in m_ProjectileModifierList)
        {
            if (item.ResetingProjectileModifier) mod = ScriptableObject.CreateInstance<ProjectileModifier>();
            mod.MovingSpeed += item.MovingSpeed;
            mod.Damage += item.Damage;
            mod.Piercing += item.Piercing;
            mod.Size += item.Size;
            mod.ExplosionRadius = item.ExplosionRadius;
            mod.NewColor = item.NewColor;
        }
        return mod;
    }

    private PlayerModifier MergeAllPlayerModifier()
    {
        PlayerModifier mod = ScriptableObject.CreateInstance<PlayerModifier>();
        mod.Rarity = 4;
        foreach (PlayerModifier item in m_PlayerModifierList)
        {
            if (item.ResetingPlayerModifier) mod = ScriptableObject.CreateInstance<PlayerModifier>();
            mod.MovementSpeed += item.MovementSpeed;
            mod.ProtectionRadius += item.ProtectionRadius;
            mod.MaxHealth += item.MaxHealth;
            mod.FireRate += item.FireRate;

            //Making sure we kept the bigger number of projectile (this modification is not adding itself up)
            mod.NumberProjectile = Mathf.Max(item.NumberProjectile, mod.NumberProjectile);
        }
        return mod;
    }
    #endregion

    #region Event / Input System
    public void OnDeath()
    {
        _isShooting = false;
        GameManager.Instance.EndGame();
    }

    #region Input System
    public void OnMove(InputValue value)
    {
        if (gm.IsGameActive)
            _movement = value.Get<Vector2>();
        else
            _movement = Vector2.zero;
    }

    public void OnShoot()
    {
        _isShooting = !_isShooting && gm.IsGameActive;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        PlayerModifierMerge = MergeAllPlayerModifier();
        Gizmos.color = new Color(1f, 0.8f, 0.1f, 0.3f);
        float corners = 4096; // How many corners the circle should have
        Vector3 origin = transform.position; // Where the circle will be drawn around
        Vector3 startRotation = transform.right * GetProtectionRadius(); // Where the first point of the circle starts
        Vector3 lastPosition = origin + startRotation;
        float angle = 0;
        while (angle <= 360)
        {
            angle += 360 / corners;
            Vector3 nextPosition = origin + (Quaternion.Euler(0, angle, 0) * startRotation);
            Gizmos.DrawLine(lastPosition, nextPosition);
            Gizmos.DrawLine(nextPosition, origin);
            lastPosition = nextPosition;
        }
    }

    #endregion
}
