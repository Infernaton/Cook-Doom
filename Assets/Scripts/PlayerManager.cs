using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class PlayerManager : LifeFormManager
{

    private Vector2 _movement;
    private Rigidbody _rigidBody;
    private float _lastSpawn;
    private GameManager gm;

    private bool _isShooting = false;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private GameObject m_Prefab;

    [SerializeField]
    private float m_FireRate;

    public float GetHealth()
    {
        return m_Health;
    }

    private void Awake()
    {
        this._rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        gm = GameManager.Instance;
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = new Vector3(m_MoveSpeed * _movement.x, _rigidBody.velocity.y, m_MoveSpeed * _movement.y);
        if (m_Health <= 0 && GameManager.Instance.IsGameActive)
        {
            _isShooting = false;
            GameManager.Instance.EndGame();
        }
    }

    void Update()
    {
        if (_isShooting && (Time.time - _lastSpawn >= 1f / m_FireRate)) SpawnProjectile();
        _updateLifeForm();
        UpdateLookAt();
    }

    private void SpawnProjectile()
    {
        GameObject proj = Instantiate(m_Prefab, transform.position, Quaternion.identity);
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

        transform.LookAt(hit.point);
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
}
