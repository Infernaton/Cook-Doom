using UnityEngine;

public enum GameState
{
    Menu, // In the gameMenu Before the game itself
    Pause, // When pausing the game, may append with input from player or the game itself (like in cinematic)
    WaitWave, // Waiting for the next wave to start with input by player
    StartWave, // Will Start the wave, will act like the Start() methods of a gameobject
    InWave, // When inside a wave
    WaitEndGame, // When no next wave but still enemies on the map
    EndGame // End game state
}

public class GameManager : MonoBehaviour
{
    public bool IsGameActive {
        get { return _currentGameState != GameState.Menu || _currentGameState != GameState.EndGame || _currentGameState != GameState.Pause; }
    }
    public float Score { get; private set; }

    private float _startTime;

    [SerializeField] PlayerController m_Player;

    [SerializeField] GameObject m_Spawner;
    [SerializeField] bool m_ActivateSpawner;
    [SerializeField] float m_TimeBeforeWave;
    [SerializeField] Wave[] m_WaveList;
    public int CurrentWaveIndex { get; private set; }

    [SerializeField] GameState _currentGameState; //Debuging only

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

    public PlayerController Player() => m_Player;

    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        CurrentWaveIndex = -1;
        _startTime = Time.time;
    }
    public void ActivateSpawner(bool activate = true)
    {
        EnemySpawnerManager e = m_Spawner.GetComponent<EnemySpawnerManager>();
        e.enabled = activate;
        if (activate) e.SetSpawnRate(m_WaveList[CurrentWaveIndex].SpawnRate);
    }

    public void Update()
    {
        switch (_currentGameState)
        {
            case GameState.StartWave:
                WillStartWave(m_TimeBeforeWave);
                break;
            case GameState.WaitEndGame:
                if (!m_Spawner.GetComponent<EnemySpawnerManager>().HasChildEnemies())
                    EndGame();
                break;
            case GameState.WaitWave:
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
        CurrentWaveIndex++;
        UIManager.Instance.MakeAnnoucement("Start of Wave " + (CurrentWaveIndex + 1));
        Invoke(nameof(StartWave), timeBefore);
        _currentGameState = GameState.InWave;
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
        if (m_WaveList.Length <= CurrentWaveIndex + 1)
        {
            Debug.Log("Wait for end game");
            _currentGameState = GameState.WaitEndGame;
            return;
        }
        Debug.Log("Display Next Button");
        UIManager.Instance.DisplayNextWaveButton();
        _currentGameState = GameState.WaitWave;
    }

    public void NextWave()
    {
        if (_currentGameState == GameState.WaitWave)
        {
            Debug.Log("Clicked");
            _currentGameState = GameState.StartWave;
            UIManager.Instance.HideNextWaveButton();
        }
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
        _currentGameState = GameState.EndGame;
        ActivateSpawner(false);
    }
}
