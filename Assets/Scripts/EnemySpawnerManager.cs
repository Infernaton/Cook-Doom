using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EnemySpawnerManager : MonoBehaviour
{
    private float _spawnRate;

    [SerializeField] float m_SpawnRange;

    public void SetSpawnRate(float spawnRate)
    {
        _spawnRate = spawnRate;
    }

    private void OnEnable()
    {
        InvokeRepeating("Spawn", 0, _spawnRate);
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
        Vector3? posSpawn = GameManager.Instance.ProtectedSpawnMob(transform.position, m_SpawnRange);
        if (posSpawn == null) return;
        instance.transform.localPosition = posSpawn.Value;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.1f, 0.1f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(m_SpawnRange, 0, m_SpawnRange));
    }
}
