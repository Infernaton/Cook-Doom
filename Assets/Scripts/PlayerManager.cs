using UnityEngine;
using UnityEngine.InputSystem;
using Entity;
using Utils;

public class PlayerManager : LifeFormManager
{

    private Vector2 _movement;
    private Rigidbody _rigidBody;
    private float _lastSpawn;
    private GameManager gm;

    private bool _isShooting = false;

    [SerializeField] float m_MoveSpeed;
    [SerializeField] GameObject m_Projectile;
    [SerializeField] float m_ProtectionRadius;

    [SerializeField] float m_FireRate;

    [SerializeField] float m_ProjSpeed;
    [SerializeField] float m_ProjDamage;
    [SerializeField] int m_ProjPiercing;

    [SerializeField] PlayerModifier[] m_PlayerModifierList;
    private PlayerModifier _playerModifierMerge;
    [SerializeField] ProjectileModifier[] m_ProjectileModifierList;
    private ProjectileModifier _projModifierMerge;

    #region get
    public float GetHealthBase()
    {
        return m_HealthBase;
    }
    public float GetCurrentHealth()
    {
        return _currentHealth;
    }
    public float GetProtectionRadius()
    {
        return Math.AddPercentage(m_ProtectionRadius, _playerModifierMerge.ProtectionRadius);
    }
    #endregion

    private void Awake()
    {
        this._rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _startLifeForm();
        gm = GameManager.Instance;
        _projModifierMerge = MergeAllProjModifier();
        _playerModifierMerge = MergeAllPlayerModifier();
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
    }

    private void SpawnProjectile()
    {
        // TODO: See if the spawn can be rotate, to avoid multi projectile in one area
        for (int i = 0; i < _playerModifierMerge.NumberProjectile; i++)
        {
            GameObject proj = Instantiate(m_Projectile, transform.position, Quaternion.identity);
            //proj.transform.SetParent(transform);

            proj.GetComponent<ProjectileManager>().Speed = Math.AddPercentage(m_ProjSpeed, _projModifierMerge.MovingSpeed);
            proj.GetComponent<ProjectileManager>().Damage = Math.AddPercentage(m_ProjDamage, _projModifierMerge.Damage);
            proj.GetComponent<ProjectileManager>().Piercing = Math.AddPercentage(m_ProjPiercing, _projModifierMerge.Piercing);

            proj.transform.localScale += _projModifierMerge.Size * proj.transform.localScale / 100;
            if (_projModifierMerge.NewColor != null) proj.GetComponent<Material>().color = (Color)_projModifierMerge.NewColor;
        }
        _lastSpawn = Time.time;
    }

    private void UpdateLookAt()
    {
        Vector2 mouse = Mouse.current.position.value;
        Vector3 startPos = new Vector3(mouse.x, mouse.y, Camera.main.nearClipPlane);

        //Get ray from mouse postion
        Ray rayCast = Camera.main.ScreenPointToRay(startPos);

        //Raycast and check if any object is hit
        Physics.Raycast(rayCast, out RaycastHit hit, Camera.main.farClipPlane);
        Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

        transform.LookAt(new Vector3(hit.point.x, 0.8f, hit.point.z));
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
            mod.CurrentHealthRecovery += item.CurrentHealthRecovery;

            mod.NumberProjectile = item.NumberProjectile;
        }
        UpdateMaxHealth(mod.MaxHealth);
        InstantHealPercent(mod.CurrentHealthRecovery);
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
