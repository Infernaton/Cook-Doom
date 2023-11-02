using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [HideInInspector] public float Speed;
    [HideInInspector] public float Damage;
    [HideInInspector] public int Piercing; //Number of target Enemy before destroyed
    [SerializeField] LayerMask m_LayerMask;

    private int _numberOfTargetHit;

    void Start()
    {
        //Maybe Rewrite this to call for Last Direction
        PlayerController p = FindAnyObjectByType<PlayerController>();

        Vector3 force = new Vector3(Speed * p.transform.forward.x, 0, Speed * p.transform.forward.z);
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    private void Update()
    {
        if (transform.position.y < -1)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision c)
    {
        //Check the layer value binary if the same as the layer mask
        if (1 << c.gameObject.layer == (m_LayerMask.value & 1 << c.gameObject.layer))
            Destroy(gameObject);

        if (c.gameObject.GetComponent<EnemyManager>())
        {
            _numberOfTargetHit++;
            EnemyManager enemy = c.gameObject.GetComponent<EnemyManager>();
            enemy.LoseHP(Damage);
            if (_numberOfTargetHit >= Piercing)
                Destroy(gameObject);
        }
    }
}
