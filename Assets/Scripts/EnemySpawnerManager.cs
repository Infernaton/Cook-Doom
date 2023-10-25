using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField]
    private float m_SpawnRate;

    [SerializeField]
    private float m_SpawnRange;

    [SerializeField]
    private GameObject[] m_EnemyType;

    /*
     * Protection radius from the player
     */

    private void OnEnable()
    {
        InvokeRepeating("Spawn", m_SpawnRate, m_SpawnRate);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Spawn()
    {
        int indexEnenmy = Random.Range(0, m_EnemyType.Length);
        GameObject instance = Instantiate(m_EnemyType[indexEnenmy], transform);
        instance.transform.localPosition = GameManager.Instance.ProtectedSpawnMob(transform.position, m_SpawnRange);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(m_SpawnRange, 0, m_SpawnRange));
    }
}
