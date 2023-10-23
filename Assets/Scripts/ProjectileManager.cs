using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private float m_Damage;

    [SerializeField]
    private LayerMask m_LayerMask;

    [SerializeField]
    private GameObject m_EnemyPrefab;

    void Start()
    {
        //Maybe Rewrite this to call for Last Direction
        PlayerManager p = FindAnyObjectByType<PlayerManager>();

        Vector3 force = new Vector3(m_Speed * p.transform.forward.x, 0, m_Speed * p.transform.forward.z);
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision c)
    {
        //Check the layer value binary if the same as the layer mask
        if (1 << c.gameObject.layer == (m_LayerMask.value & 1 << c.gameObject.layer))
            Destroy(gameObject);

        if (c.gameObject.GetComponent<EnemyManager>())
        {
            print("Hit Ennemy");
            EnemyManager enemy = c.gameObject.GetComponent<EnemyManager>();
            enemy.LoseHP(m_Damage);
        }
    }
}
