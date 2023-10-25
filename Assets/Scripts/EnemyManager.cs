using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private GameObject _target;

    [SerializeField]
    private float m_Health;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private Target m_Target;

    public float damage;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _target = TargetManager.Instance.GetGameObject(m_Target);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameActive)
        {
            transform.position += transform.forward * m_MoveSpeed * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_target.transform);
    }

    public void LoseHP(float loosedHealth)
    {
        m_Health -= loosedHealth;

        if (m_Health <= 0) 
        {
            Destroy(gameObject);
        }
    }
}
