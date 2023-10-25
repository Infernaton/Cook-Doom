using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EnemySpawnerManager : MonoBehaviour
{
    private float _spawnRate;

    [SerializeField]
    private float m_SpawnRange;

    public void SetSpawnRate(float spawnRate)
    {
        _spawnRate = spawnRate;
    }

    private void OnEnable()
    {
        InvokeRepeating("Spawn", _spawnRate, _spawnRate);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Spawn()
    {
        GameObject[] enemyList = GameManager.Instance.GetCurrentWaveMobList();
        int indexEnenmy = Random.Range(0, enemyList.Length);
        GameObject instance = Instantiate(enemyList[indexEnenmy], transform);
        instance.transform.localPosition = GameManager.Instance.ProtectedSpawnMob(transform.position, m_SpawnRange);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(m_SpawnRange, 0, m_SpawnRange));
    }
}
