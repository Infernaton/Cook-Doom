using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class Menu : MonoBehaviour
{
    [SerializeField] RawImage m_BlackScreen;

    public void Start()
    {
        StartCoroutine(Anim.FadeOut(1f, m_BlackScreen));
    }
    public void Play()
    {
        StartCoroutine(FadeTransition("MainScene"));
    }

    public void MainMenu()
    {
        StartCoroutine(FadeTransition("MenuScene"));
    }

    public void LeaderBoard()
    {
        StartCoroutine(FadeTransition("LeaderboardScene"));
    }

    IEnumerator FadeTransition(string scene)
    {
        StartCoroutine(Anim.FadeIn(1f, m_BlackScreen));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        //Quit app
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
