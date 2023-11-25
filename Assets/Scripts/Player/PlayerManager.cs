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
    [SerializeField] GameObject m_Projectile;
    [SerializeField] Transform m_SpawnProjectile;
    [SerializeField] float m_ProtectionRadius;

    [SerializeField] float m_FireRate;

    [SerializeField] float m_ProjSpeed;
    [SerializeField] float m_ProjDamage;
    [SerializeField] int m_ProjPiercing;

    [SerializeField] List<PlayerModifier> m_PlayerModifierList;
    private PlayerModifier _playerModifierMerge;
    [SerializeField] List<ProjectileModifier> m_ProjectileModifierList;
    private ProjectileModifier _projModifierMerge;

    #region get
    public float GetProtectionRadius()
    {
        return Math.AddPercentage(m_ProtectionRadius, _playerModifierMerge.ProtectionRadius);
    }

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
                float addedHealth = Math.Percentage(m_HealthBase, playerMod.MaxHealth);
                _currentMaxHealth += addedHealth;
                InstantHeal(addedHealth);
            }

            m_PlayerModifierList.Add(playerMod);
            _playerModifierMerge = MergeAllPlayerModifier();
        } 
        else if (mod is ProjectileModifier projMod)
        {
            m_ProjectileModifierList.Add(projMod);
            _projModifierMerge = MergeAllProjModifier();
        }
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _projModifierMerge = MergeAllProjModifier();
        _playerModifierMerge = MergeAllPlayerModifier();
    }

    private void Start()
    {
        _startLifeForm();
        gm = GameManager.Instance;
    }

    private void FixedUpdate()
    {
        float moveSpeed = Math.AddPercentage(m_MoveSpeed, _playerModifierMerge.MovementSpeed);
        _rigidBody.velocity = new Vector3(moveSpeed * _movement.x, _rigidBody.velocity.y, moveSpeed * _movement.y);
    }

    void Update()
    {
        if (_isShooting && (Time.time - _lastSpawn >= 1f / Math.AddPercentage(m_FireRate, _playerModifierMerge.FireRate))) SpawnProjectile();
        _updateLifeForm();
        UpdateLookAt();

        if (transform.position.y < -1)
            gm.EndGame();
    }

    private void SpawnProjectile()
    {
        for (int i = 0; i < _playerModifierMerge.NumberProjectile; i++)
        {
            //Set the projectile all arround the player when getting a bonus
            float degree = (360 / _playerModifierMerge.NumberProjectile) * i;
            GameObject proj = Instantiate(m_Projectile, m_SpawnProjectile.position, Quaternion.AngleAxis(degree, transform.up) * transform.rotation);
            proj.transform.SetParent(transform.parent, true);

            ProjectileManager projManager = proj.GetComponent<ProjectileManager>();
            projManager.Speed = Math.AddPercentage(m_ProjSpeed, _projModifierMerge.MovingSpeed);
            projManager.Damage = Math.AddPercentage(m_ProjDamage, _projModifierMerge.Damage);
            projManager.Piercing = Math.AddPercentage(m_ProjPiercing, _projModifierMerge.Piercing);

            proj.transform.localScale += _projModifierMerge.Size * proj.transform.localScale / 100;
            if (_projModifierMerge.NewColor != null) proj.GetComponent<Material>().color = (Color)_projModifierMerge.NewColor;
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
        _projModifierMerge = MergeAllProjModifier();
        _playerModifierMerge = MergeAllPlayerModifier();
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
