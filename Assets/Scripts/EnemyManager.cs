using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Rigidbody _rigidBody;

    [SerializeField]
    private float m_Health;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private Target m_Target;
    // Start is called before the first frame update
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * m_MoveSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(TargetManager.Instance.GetGameObject(m_Target).transform);
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
