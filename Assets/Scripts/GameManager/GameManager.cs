using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    public bool IsGameActive { get; private set; }

    private float _startTime;
    private float _score;

    [SerializeField]
    private GameObject m_Player;

    [SerializeField]
    private GameObject m_Spawner;

    [SerializeField]
    private bool m_ActivateSpawner;

    [SerializeField]
    private float m_ProtectionRadius;

    [SerializeField] private TMP_Text m_Health;
    [SerializeField] private TMP_Text m_Time;
    [SerializeField] private TMP_Text m_Score;

    [SerializeField] private GameObject m_SpawnerObject;
    [SerializeField] private Wave[] m_WaveList;
    private int _currentWaveIndex;

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
        _score = 0;
        _currentWaveIndex = 0;
        _startTime = Time.time;
        Invoke("StartWave", 3f);
    }
    public void ActivateSpawner(bool activate = true)
    {
        EnemySpawnerManager e = m_Spawner.GetComponent<EnemySpawnerManager>();
        m_ActivateSpawner = activate;
        e.SetSpawnRate(m_WaveList[_currentWaveIndex].SpawnRate);
        e.enabled = activate;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameActive)
        {
            float time = Time.time - _startTime;
            UpdateTime(time);
            UpdateScore();
        }
        UpdateHealth();
    }

    #region Update UI
    void UpdateHealth()
    {
        m_Health.text = string.Format("Health: {0:0}", m_Player.GetComponent<PlayerManager>().GetHealth());
    }

    void UpdateScore()
    {
        m_Score.text = string.Format("Score: {0:0}", _score);
    }

    private void UpdateTime(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        m_Time.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    #endregion

    public Vector3 ProtectedSpawnMob(Vector3 pos, float spawnRange)
    {
        Vector3 finalPos = Area.GetRandomCoord(pos, new Vector3(spawnRange, 0, spawnRange));
        if (Vector3.Distance(m_Player.transform.position, finalPos) < m_ProtectionRadius) return ProtectedSpawnMob(pos, spawnRange);
        return finalPos;
    }

    public GameObject[] GetCurrentWaveMobList()
    {
        return m_WaveList[_currentWaveIndex].MobList;
    }

    public void StartWave()
    {
        Debug.Log("Start Wave " + _currentWaveIndex);
        ActivateSpawner();
        Invoke(nameof(StopWave), m_WaveList[_currentWaveIndex].WaveDuration);
    }

    private void StopWave()
    {
        ActivateSpawner(false);
        Debug.Log("Stop Wave " + _currentWaveIndex);
        _currentWaveIndex++;
        if (m_WaveList.Length < _currentWaveIndex + 1) return;
        Invoke(nameof(StartWave), 5f);
    }

    public void IncrementScore()
    {
        _score++;
    }

    public void EndGame()
    {
        IsGameActive = false;
        ActivateSpawner(false);
    }
}
