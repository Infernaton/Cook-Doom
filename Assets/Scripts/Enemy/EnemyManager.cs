using UnityEngine;
using Entity;
using Utils;

public class EnemyManager : LifeFormManager
{
    private GameObject _target;

    [SerializeField] float m_MoveSpeed;
    [SerializeField] Target m_Target;
    [SerializeField] int m_GoldReward;
    [SerializeField] float m_Damage;

    private bool _onTarget;

    void Awake()
    {
        _target = TargetManager.Instance.GetGameObject(m_Target);
    }

    private void Start()
    {
        _startLifeForm();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameActive && !_onTarget)
        {
            transform.position += m_MoveSpeed * Time.deltaTime * transform.forward;
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
            _onTarget = true;
        }
    }
    private void OnCollisionExit(Collision c)
    {
        if (Compare.GameObjects(c.gameObject, _target))
        {
            _onTarget = false;
        }
    }

    public void OnDeath()
    {
        Destroy(gameObject);
        GameManager.Instance.IncrementScore(m_GoldReward);
    }
}
