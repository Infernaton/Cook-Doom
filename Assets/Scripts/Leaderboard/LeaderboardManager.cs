using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] RectTransform m_LeaderboardList;
    [SerializeField] ScoreUI m_ScoreUIPrefab;

    // Start is called before the first frame update
    void Start()
    {
        JsonFile.FinalScoreList list = JsonFile.GetStoredData(Constants.GetPathFinalScore());

        for (int i = 0; i < list.Scores.Count; i++)
        {
            ScoreUI sc = Instantiate(m_ScoreUIPrefab, m_LeaderboardList.transform);
            sc.SetTexts(list.Scores[i]);
        }
    }
}
