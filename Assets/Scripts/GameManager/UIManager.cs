using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIManager : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] TMP_Text m_HealthUI;
    [SerializeField] TMP_Text m_TimeUI;
    [SerializeField] TMP_Text m_ScoreUI;
    [SerializeField] TMP_Text m_WaveUI;
    [SerializeField] TMP_Text m_Announcement;
    [SerializeField] TMP_Text m_Tips;
    [SerializeField] CanvasGroup m_NextWave;

    public static UIManager Instance; // A static reference to the UIManager instance
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
    void Start()
    {
        _gm = GameManager.Instance;
        m_Announcement.enabled = false;
        m_Tips.enabled = false;
        m_NextWave.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.IsGameActive)
        {
            UpdateTime();
            UpdateScore();
            UpdateWave();
        }
        UpdateHealth();
    }

    #region Update UI
    void UpdateHealth()
    {
        PlayerController p = _gm.Player();
        m_HealthUI.text = string.Format("Health: {0:0} / {1:0}", p.GetCurrentHealth(), p.GetHealthBase());
    }

    void UpdateScore()
    {
        m_ScoreUI.text = string.Format("VegeScore: {0:0}", _gm.Score);
    }

    // Since we pass wave using a entry input, it's not necessary to display the time played
    private void UpdateTime()
    {
        float time = _gm.GetActiveTime();
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        //m_TimeUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateWave()
    {
        m_WaveUI.text = string.Format("Wave: {0:0}", _gm.CurrentWaveIndex + 1);
    }
    #endregion

    public void MakeAnnoucement(string message)
    {
        m_Announcement.enabled = true;
        m_Announcement.text = message;
        StartCoroutine(Anim.FadeOut(3f, m_Announcement));
    }

    public void MakeTips(string message)
    {
        m_Tips.text = message;
        StartCoroutine(Anim.FadeIn(2f, m_Tips));
    }

    public void HideTips()
    {
        StartCoroutine(Anim.FadeOut(0.5f, m_Tips));
    }

    public void DisplayNextWaveButton()
    {
        StartCoroutine(Anim.FadeIn(1f, m_NextWave));
    }

    public void HideNextWaveButton()
    {
        StartCoroutine(Anim.FadeOut(0.5f, m_NextWave));
    }
}
