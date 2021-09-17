using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public string firstMap;

    public void startGameButton()
    {
        SceneManager.LoadScene(firstMap, LoadSceneMode.Single);
    }

    public void quitGameButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
