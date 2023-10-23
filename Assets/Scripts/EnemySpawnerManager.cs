using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    [SerializeField]
    private float m_SpawnRate;

    [SerializeField]
    private GameObject[] m_EnemyType;

    /*
     * Set Gizmo
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
        GameObject instance = Instantiate(m_EnemyType[0], transform.position, Quaternion.identity);
        instance.transform.SetParent(transform, true);
        instance.transform.localPosition = GameManager.Instance.GetRandomCoordInArea(transform.position, new Vector3(20, 1, 20));
    }
}
