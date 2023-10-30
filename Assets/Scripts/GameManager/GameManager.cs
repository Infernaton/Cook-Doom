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

    [SerializeField] GameObject m_Player;

    [SerializeField] GameObject m_Spawner;

    [SerializeField] TMP_Text m_HealthUI;
    [SerializeField] TMP_Text m_TimeUI;
    [SerializeField] TMP_Text m_ScoreUI;
    [SerializeField] TMP_Text m_WaveUI;

    [SerializeField] bool m_ActivateSpawner;
    [SerializeField] private GameObject m_SpawnerObject;
    [SerializeField] Wave[] m_WaveList;
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

    public GameObject Player()
    {
        return m_Player;
    }

    // Start is called before the first frame update
    void Start()
    {
        IsGameActive = true;
        _score = 0;
        _currentWaveIndex = -1;
        _startTime = Time.time;
        Invoke("StartWave", 3f);
    }
    public void ActivateSpawner(bool activate = true)
    {
        EnemySpawnerManager e = m_Spawner.GetComponent<EnemySpawnerManager>();
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
            UpdateWave();
        }
        UpdateHealth();
    }

    #region Update UI
    void UpdateHealth()
    {
        m_HealthUI.text = string.Format("Health: {0:0}", m_Player.GetComponent<PlayerManager>().GetCurrentHealth());
    }

    void UpdateScore()
    {
        m_ScoreUI.text = string.Format("VegeScore: {0:0}", _score);
    }

    private void UpdateTime(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        m_TimeUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateWave()
    {
        m_WaveUI.text = string.Format("Wave: {0:0}", _currentWaveIndex + 1);
    }
    #endregion

    #region Spawning Mob
    public GameObject[] GetCurrentWaveMobList()
    {
        return m_WaveList[_currentWaveIndex].MobList;
    }

    public void WillStartWave()
    {
        Invoke(nameof(StartWave), 5f);
    }

    public void StartWave()
    {
        if (!m_ActivateSpawner) return;
        _currentWaveIndex++;
        if (m_WaveList.Length < _currentWaveIndex + 1) return;
        ActivateSpawner();
        Invoke(nameof(StopWave), m_WaveList[_currentWaveIndex].WaveDuration);
    }

    private void StopWave()
    {
        ActivateSpawner(false);
        WillStartWave(); // Will be activate using a bind key
    }
    #endregion

    public void IncrementScore(float increment)
    {
        _score += increment;
    }

    public void EndGame()
    {
        Debug.Log("End Game");
        IsGameActive = false;
        ActivateSpawner(false);
    }
}
