using UnityEngine;

public enum GameState
{
    Menu, // In the gameMenu Before the game itself
    WaitWave, // Waiting for the next wave to start with input by player
    StartWave, // Will Start the wave, will act like the Start() methods of a gameobject
    InWave, // When inside a wave
    EndGame // End game state
}

public class GameManager : MonoBehaviour
{
    public bool IsGameActive { get; private set; }
    public float Score { get; private set; }

    private float _startTime;

    [SerializeField] GameObject m_Player;

    [SerializeField] GameObject m_Spawner;
    [SerializeField] bool m_ActivateSpawner;
    [SerializeField] float m_TimeBeforeWave;
    [SerializeField] Wave[] m_WaveList;
    public int CurrentWaveIndex { get; private set; }

    public GameState CurrentGameState;

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
    }
    public void ActivateSpawner(bool activate = true)
    {
        EnemySpawnerManager e = m_Spawner.GetComponent<EnemySpawnerManager>();
        e.SetSpawnRate(m_WaveList[CurrentWaveIndex].SpawnRate);
        e.enabled = activate;
    }

    public void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.WaitWave:
                break;
            case GameState.StartWave:
                WillStartWave(m_TimeBeforeWave);
                break;
            case GameState.InWave:
            default: break;
        }
    }

    public float GetActiveTime()
    {
        return Time.time - _startTime;
    }

    #region Spawning Mob
    public GameObject[] GetCurrentWaveMobList() => m_WaveList[CurrentWaveIndex].MobList;

    public void WillStartWave(float timeBefore)
    {
        UIManager.Instance.HideTips();
        CurrentWaveIndex++;
        UIManager.Instance.MakeAnnoucement("Start of Wave " + (CurrentWaveIndex + 1));
        Invoke(nameof(StartWave), timeBefore);
        CurrentGameState = GameState.InWave;
    }

    public void StartWave()
    {
        if (!m_ActivateSpawner) return;
        ActivateSpawner();
        Invoke(nameof(StopWave), m_WaveList[CurrentWaveIndex].WaveDuration);
    }

    private void StopWave()
    {
        ActivateSpawner(false);
        if (m_WaveList.Length < CurrentWaveIndex + 1)
        { 
            EndGame();
            return; 
        }
        UIManager.Instance.MakeTips("Tap 'N' to start the next wave !");
        CurrentGameState = GameState.WaitWave;
        //Invoke("WillStartWave", 1f); // Will be activate using a bind key
    }
    #endregion

    public void IncrementScore(float increment)
    {
        Score += increment;
    }

    public void EndGame()
    {
        UIManager.Instance.MakeAnnoucement("Good Job !");
        Debug.Log("End Game");
        CancelInvoke();
        IsGameActive = false;
        CurrentGameState = GameState.EndGame;
        ActivateSpawner(false);
    }
}
