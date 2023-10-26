using UnityEngine;
using Entity;
using Utils;

public class EnemyManager : LifeFormManager
{
    private Rigidbody _rigidBody;
    private GameObject _target;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private Target m_Target;

    [SerializeField]
    private float m_GoldReward;

    [SerializeField]
    private float m_Damage;

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
        if (GameManager.Instance.IsGameActive)
        {
            transform.LookAt(_target.transform);
            _updateLifeForm();
        }
    }
    private void OnCollisionEnter(Collision c)
    {
        if (Compare.GameObjects(c.gameObject, _target))
        {
            c.gameObject.GetComponent<LifeFormManager>().LoseHP(m_Damage);
        }
    }

    public void OnDeath()
    {
        Destroy(gameObject);
        GameManager.Instance.IncrementScore(m_GoldReward);
    }
}
