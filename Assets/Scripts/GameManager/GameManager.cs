using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    public bool IsGameActive { get; private set; }

    private float _startTime;
    public float Score { get; private set; }

    [SerializeField] GameObject m_Player;

    [SerializeField] GameObject m_Spawner;

    [SerializeField] bool m_ActivateSpawner;
    [SerializeField] private GameObject m_SpawnerObject;
    [SerializeField] Wave[] m_WaveList;
    public int CurrentWaveIndex { get; private set; }

    public static GameManager Instance; // A static reference to the GameManager instance
    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
    }

    public GameObject Player() => m_Player;

    // Start is called before the first frame update
    void Start()
    {
        IsGameActive = true;
        Score = 0;
        CurrentWaveIndex = -1;
        _startTime = Time.time;
        WillStartWave(3f);
    }
    public void ActivateSpawner(bool activate = true)
    {
        EnemySpawnerManager e = m_Spawner.GetComponent<EnemySpawnerManager>();
        e.SetSpawnRate(m_WaveList[CurrentWaveIndex].SpawnRate);
        e.enabled = activate;
    }

    public float GetActiveTime()
    {
        return Time.time - _startTime;
    }

    #region Spawning Mob
    public GameObject[] GetCurrentWaveMobList() => m_WaveList[CurrentWaveIndex].MobList;

    public void WillStartWave() => WillStartWave(4f);
    public void WillStartWave(float timeBefore)
    {
        UIManager.Instance.MakeAnnoucement("Start of Wave " + (CurrentWaveIndex + 2));
        Invoke(nameof(StartWave), timeBefore);
    }

    public void StartWave()
    {
        if (!m_ActivateSpawner) return;
        CurrentWaveIndex++;
        if (m_WaveList.Length < CurrentWaveIndex + 1) return;
        ActivateSpawner();
        Invoke(nameof(StopWave), m_WaveList[CurrentWaveIndex].WaveDuration);
    }

    private void StopWave()
    {
        ActivateSpawner(false);
        Invoke("WillStartWave", 1f); // Will be activate using a bind key
    }
    #endregion

    public void IncrementScore(float increment)
    {
        Score += increment;
    }

    public void EndGame()
    {
        Debug.Log("End Game");
        IsGameActive = false;
        ActivateSpawner(false);
    }
}
