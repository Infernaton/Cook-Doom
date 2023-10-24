using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    public bool IsGameActive { get; private set; }

    private float _startTime;

    [SerializeField]
    private GameObject m_Player;

    [SerializeField]
    private GameObject m_Spawner;

    [SerializeField]
    private bool m_ActivateSpawner = true;

    [SerializeField]
    private float m_ProtectionRadius;

    [SerializeField] private TMP_Text m_Health;
    [SerializeField] private TMP_Text m_Time;
    [SerializeField] private TMP_Text m_Score;

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
        IsGameActive = true;
        _startTime = Time.time;
        ActivateSpawner(m_ActivateSpawner);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameActive)
        {
            float time = Time.time - _startTime;
            UpdateTime(time);
        }
        UpdateHealth();
    }

    void UpdateHealth()
    {
        m_Health.text = string.Format("Health: {0:0}", m_Player.GetComponent<PlayerManager>().GetHealth());
    }

    private void UpdateTime(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        m_Time.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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

    public void EndGame()
    {
        IsGameActive = false;
    }
}
