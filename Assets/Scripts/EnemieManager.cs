using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieManager : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private TargetManager _targetManager;

    [SerializeField]
    private float m_HealtBar;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private Target m_Target;
    // Start is called before the first frame update
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _targetManager = TargetManager.Instance;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * m_MoveSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_targetManager.GetGameObject(m_Target).transform);
    }
}
