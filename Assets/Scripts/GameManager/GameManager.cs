using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Spawner;

    [SerializeField]
    private bool m_ActivateSpawner = true;

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

    public Vector3 GetRandomCoordInArea(Vector3 center, Vector3 scale)
    {
        Bounds b = new Bounds(center, scale);
        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y),
            Random.Range(b.min.z, b.max.z)
        );
    }
}
