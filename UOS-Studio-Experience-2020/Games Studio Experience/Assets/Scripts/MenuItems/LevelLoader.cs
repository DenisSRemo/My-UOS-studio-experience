using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private bool loadingLevel;

    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(int LevelIndex)
    {
        var LI = LoadIt(LevelIndex);
        StartCoroutine(LI);
    }

    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else 
        { 
            //Loads menu, reached end of level
            SceneManager.LoadScene(0); 
        }
    }

    private IEnumerator LoadIt(int LevelIndex)
    {
        if (loadingLevel) yield break;

        loadingLevel = true;

        var DoStuff = SceneManager.LoadSceneAsync(LevelIndex);

        DoStuff.allowSceneActivation = false;
        while (!DoStuff.isDone)
        {
            //put your loading screen and stuff here
            yield return new WaitForSeconds(0.1f);

            if (DoStuff.progress >= 0.9f) DoStuff.allowSceneActivation = true;

            Debug.Log(DoStuff.progress);
        }

        loadingLevel = false;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}