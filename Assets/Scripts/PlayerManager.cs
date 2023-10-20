using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    private Vector2 _movement;
    public Vector2 lastDirection;
    private bool _isAlive = true;
    private Rigidbody _rigidBody;
    private float _lastSpawn;

    private bool _isShooting = false;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private GameObject m_Prefab;

    [SerializeField]
    private float m_Rate;

    private void Awake()
    {
        this._rigidBody = GetComponent<Rigidbody>();
        lastDirection = new Vector2(-1, 0);
    }

    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        _rigidBody.velocity = new Vector3(m_MoveSpeed * _movement.x, _rigidBody.velocity.y, m_MoveSpeed * _movement.y);
    }

    void Update()
    {
        if (_isShooting && (Time.time - _lastSpawn >= 1f / m_Rate))
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        Instantiate(m_Prefab, transform);
        _lastSpawn = Time.time;
    }

    public void OnMove(InputValue value)
    {
        if (_isAlive)
        {
            _movement = value.Get<Vector2>();
            if (_movement != Vector2.zero)
            {
                lastDirection = _movement;
            }
        }
        else
            _movement = Vector2.zero;
    }

    public void OnShoot()
    {
        _isShooting = !_isShooting;
    }
}
