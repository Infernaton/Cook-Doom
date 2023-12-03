using UnityEngine;
using Utils;

public class ProjectileManager : MonoBehaviour
{
    [HideInInspector] public float Speed;
    [HideInInspector] public float Damage;
    [HideInInspector] public int Piercing; //Number of target Enemy before destroyed
    [SerializeField] LayerMask m_LayerMask;

    private int _numberOfTargetHit;
    private GameObject _lastHitObject;

    void Start()
    {
        //Maybe Rewrite this to call for Last Direction
        //PlayerController p = GameManager.Instance.Player();
        _lastHitObject = GameManager.Instance.Player().gameObject; // To make _lastHitObject not null

        Vector3 force = new Vector3(Speed * transform.forward.x, 0, Speed * transform.forward.z);
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

        if (c.gameObject.GetComponent<EnemyManager>() && !Compare.GameObjects(_lastHitObject, c.gameObject))
        {
            _numberOfTargetHit++;
            _lastHitObject = c.gameObject;
            EnemyManager enemy = c.gameObject.GetComponent<EnemyManager>();
            enemy.LoseHP(Damage);
            Debug.Log("Hit: " + _numberOfTargetHit + " | Piercing: " + Piercing);
            if (_numberOfTargetHit >= Piercing)
                Destroy(gameObject);
        }
    }
}
