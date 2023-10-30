using UnityEngine;
using UnityEngine.InputSystem;
using Entity;

public class PlayerManager : LifeFormManager
{

    private Vector2 _movement;
    private Rigidbody _rigidBody;
    private float _lastSpawn;
    private GameManager gm;

    private bool _isShooting = false;

    [SerializeField] float m_MoveSpeed;
    [SerializeField] GameObject m_Projectile;
    [SerializeField] float m_FireRate;
    [SerializeField] float m_ProtectionRadius;

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
        return m_ProtectionRadius;
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
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = new Vector3(m_MoveSpeed * _movement.x, _rigidBody.velocity.y, m_MoveSpeed * _movement.y);
    }

    void Update()
    {
        if (_isShooting && (Time.time - _lastSpawn >= 1f / m_FireRate)) SpawnProjectile();
        _updateLifeForm();
        UpdateLookAt();
    }

    private void SpawnProjectile()
    {
        GameObject proj = Instantiate(m_Projectile, transform.position, Quaternion.identity);
        //proj.transform.SetParent(transform);
        _lastSpawn = Time.time;
    }

    private void UpdateLookAt()
    {
        Vector2 mouse = Mouse.current.position.value;
        Vector3 startPos = new Vector3(mouse.x, mouse.y, Camera.main.nearClipPlane);

        //Get ray from mouse postion
        RaycastHit hit;
        Ray rayCast = Camera.main.ScreenPointToRay(startPos);

        //Raycast and check if any object is hit
        Physics.Raycast(rayCast, out hit, Camera.main.farClipPlane);
        Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

        transform.LookAt(new Vector3(hit.point.x, 0.8f, hit.point.z));
    }

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
        Vector3 startRotation = transform.right * m_ProtectionRadius; // Where the first point of the circle starts
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
}
