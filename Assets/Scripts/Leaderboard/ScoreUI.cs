using TMPro;
using UnityEngine;
using Utils;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text m_Time;
    [SerializeField] TMP_Text m_PlayerName;
    [SerializeField] TMP_Text m_Wave;
    [SerializeField] TMP_Text m_Score;
    public void SetTexts(FinalScore fs)
    {
        m_Time.text = DMath.TimeToString(fs.Time);
        m_Wave.text = fs.Wave.ToString();
        m_Score.text = fs.VegeScore.ToString();

        m_PlayerName.text = "Player"; // No name is registered for now
    }
}
