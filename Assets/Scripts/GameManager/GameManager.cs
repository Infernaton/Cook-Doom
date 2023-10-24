using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Player;

    [SerializeField]
    private GameObject m_Spawner;

    [SerializeField]
    private bool m_ActivateSpawner = true;

    [SerializeField]
    private float m_ProtectionRadius;

    public static GameManager Instance; // A static reference to the GameManager instance

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ActivateSpawner(m_ActivateSpawner);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateSpawner(bool activate = true)
    {
        EnemySpawnerManager e = m_Spawner.GetComponent<EnemySpawnerManager>();
        m_ActivateSpawner = activate;
        e.enabled = m_ActivateSpawner;
    }

    public Vector3 ProtectedSpawnMob(Vector3 pos, float spawnRange)
    {
        Vector3 finalPos = Area.GetRandomCoord(pos, new Vector3(spawnRange, 0, spawnRange));
        if (Vector3.Distance(m_Player.transform.position, finalPos) < m_ProtectionRadius) return ProtectedSpawnMob(pos, spawnRange);
        return finalPos;
    }
}
