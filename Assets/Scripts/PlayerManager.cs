using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    private Vector2 _movement;
    private bool _isAlive = true;
    private Rigidbody _rigidBody;
    private float _lastSpawn;
    private float _startInvincibility;

    private bool _isShooting = false;

    [SerializeField]
    private float m_MoveSpeed;
    [SerializeField] private float m_Health;
    [SerializeField] private float m_InvincibiltyTime;

    [SerializeField]
    private GameObject m_Prefab;

    [SerializeField]
    private float m_FireRate;

    private void Awake()
    {
        this._rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = new Vector3(m_MoveSpeed * _movement.x, _rigidBody.velocity.y, m_MoveSpeed * _movement.y);
    }

    void Update()
    {
        if (_startInvincibility >= 0f) _startInvincibility -= Time.deltaTime;
        if (_isShooting && (Time.time - _lastSpawn >= 1f / m_FireRate))
        {
            Spawn();
        }
        UpdateLookAt();
    }

    private void Spawn()
    {
        Instantiate(m_Prefab, transform.position, Quaternion.identity);
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

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.GetComponent<EnemyManager>() && _startInvincibility <= 0f)
        {
            LoseHP(c.gameObject.GetComponent<EnemyManager>().damage);
        }
    }

    #region Input System
    public void OnMove(InputValue value)
    {
        if (_isAlive)
            _movement = value.Get<Vector2>();
        else
            _movement = Vector2.zero;
    }

    public void OnShoot()
    {
        if (_isAlive)
            _isShooting = !_isShooting;
        else
            _isShooting = false;
    }
    #endregion

    public void LoseHP(float loosedHealth)
    {
        m_Health -= loosedHealth;
        _startInvincibility = m_InvincibiltyTime;

        if (m_Health <= 0)
        {
            _isAlive = false;
        }
    }
}
