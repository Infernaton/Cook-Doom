using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieManager : MonoBehaviour
{
    private Rigidbody _rigidBody;

    [SerializeField]
    private float m_HealtBar;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private GameObject m_Target;
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
        transform.LookAt(m_Target.transform);
    }
}
