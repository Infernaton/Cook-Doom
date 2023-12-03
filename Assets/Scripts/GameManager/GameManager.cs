using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public enum GameState
{
    Menu, // In the gameMenu Before the game itself
    Pause, // When pausing the game, may append with input from player or the game itself (like in cinematic)
    WaitNextWave, // Waiting for the next wave to start with input by player
    StartWave, // Will Start the wave, will act like the Start() methods of a gameobject
    InWave, // When inside a wave
    WaitEndWave, // When no next wave but still enemies on the map
    EndGame // End game state
}

[Serializable]
public class FinalScore
{
    public int VegeScore;
    public string PlayerName;
    public int Wave;
    public float Time;
}

public class GameManager : MonoBehaviour
{
    public bool IsGameActive {
        get { return _currentGameState != GameState.Menu && _currentGameState != GameState.EndGame && _currentGameState != GameState.Pause; }
    }
    public bool IsWaitingWave
    {
        get { return _currentGameState == GameState.WaitNextWave; }
    }
    public int Score { get; private set; }

    private float _startTime;
    private GameState _currentGameState;

    [SerializeField] PlayerController m_Player;

    [SerializeField] EnemySpawnerManager m_Spawner;
    [SerializeField] bool m_ActivateSpawner;
    [SerializeField] float m_TimeBeforeWave;
    [SerializeField] List<GameObject> m_MobList;
    [SerializeField] ItemHolder m_ItemHolderPrefab;
    [SerializeField] Transform[] m_ItemSpawnPoints;

    [SerializeField] GameObject m_CanvaEndGame;

    public int CurrentWaveIndex { get; private set; }
    public Wave CurrentWave { get; private set; }

    public static GameManager Instance; // A static reference to the GameManager instance

    #region Getter
    public PlayerController Player() => m_Player;

    public float GetActiveTime() => Time.time - _startTime;

    public List<GameObject> GetCurrentWaveMobList() => CurrentWave.MobList;
    #endregion

    #region Setter
    public void IncrementScore(int increment)
    {
        Score += increment;
    }
    #endregion

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
    }

    void Start()
    {
        Score = 0;
        CurrentWaveIndex = 0;
        _startTime = Time.time;
        _currentGameState = GameState.StartWave;
    }
    public void ActivateSpawner(bool activate = true)
    {
        m_Spawner.enabled = activate;
        if (activate) m_Spawner.SetSpawnRate(CurrentWave.SpawnRate);
    }

    public void Update()
    {
        switch (_currentGameState)
        {
            case GameState.StartWave:
                WillStartWave(m_TimeBeforeWave);
                break;
            case GameState.WaitEndWave:
                if (!m_Spawner.GetComponent<EnemySpawnerManager>().HasChildEnemies())
                    EndWave();
                break;
            case GameState.WaitNextWave:
            case GameState.InWave:
            default: break;
        }
    }

    #region Wave Handle
    private void GenerateWave()
    {
        Wave wave = ScriptableObject.CreateInstance<Wave>();
        float spawnrate = 1f - (CurrentWaveIndex / 20f);
        wave.SpawnRate = Mathf.Max(0.1f, spawnrate);
        wave.WaveDuration = 5f + (CurrentWaveIndex / 2f);
        wave.MobList = m_MobList;
        //for (int i = 0; i < m_MobList.Count; i++)
        //{
        //    //print(CurrentWaveIndex + " >= " + (i * i));
        //    //if (CurrentWaveIndex-1 >= i * i) break;
        //    print(m_MobList[i].name);
        //    wave.MobList.Append(m_MobList[i]);
        //    print(wave.MobList);
        //}
        CurrentWave = wave;
    }
    private void WillStartWave(float timeBefore)
    {
        KillItemHolder();
        CurrentWaveIndex++;
        GenerateWave();
        UIManager.Instance.MakeAnnoucement("Start of Wave " + (CurrentWaveIndex));
        Invoke(nameof(StartWave), timeBefore);
        _currentGameState = GameState.InWave;
    }

    private void StartWave()
    {
        if (!m_ActivateSpawner) return;
        ActivateSpawner();
        Invoke(nameof(StopSpawn), CurrentWave.WaveDuration);
    }

    private void StopSpawn()
    {
        ActivateSpawner(false);
        Debug.Log("Wait for killing all mobs");
        _currentGameState = GameState.WaitEndWave;
    }

    private void EndWave()
    {
        UIManager.Instance.DisplayNextWaveButton();
        SpawnNewItemHolder();
        _currentGameState = GameState.WaitNextWave;
    }

    // Click on NextWaveButton
    public void NextWave()
    {
        if (_currentGameState == GameState.WaitNextWave)
        {
            _currentGameState = GameState.StartWave;
            UIManager.Instance.HideNextWaveButton();
        }
    }
    #endregion

    [ContextMenu("Spawn Item Holder")]
    private void SpawnNewItemHolder()
    {
        KillItemHolder();
        for (int i = 0; i < m_ItemSpawnPoints.Length; i++)
        {
            ItemHolder itemHolder = Instantiate(m_ItemHolderPrefab, m_ItemSpawnPoints[i], true);
            itemHolder.transform.localPosition = Vector3.zero;
        }
    }
    [ContextMenu("Kill Items Holders")]
    private void KillItemHolder()
    {
        for(int i = 0; i < m_ItemSpawnPoints.Length; i++)
        {
            for(int o=0; o < m_ItemSpawnPoints[i].childCount; o++)
            {
                Destroy(m_ItemSpawnPoints[i].GetChild(o).gameObject);
            }
        }
    }

    public void EndGame()
    {
        UIManager.Instance.MakeAnnoucement("Good Job !");
        CancelInvoke();
        _currentGameState = GameState.EndGame;
        ActivateSpawner(false);
        m_CanvaEndGame.SetActive(true);
        FinalScore fs = new()
        {
            VegeScore = Score,
            Time = GetActiveTime(),
            Wave = CurrentWaveIndex,
            PlayerName = "Player"
        };
        JsonFile.AddData(Constants.GetPathFinalScore(), fs);
    }
}
