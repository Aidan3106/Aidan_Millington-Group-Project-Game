using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controlsMenu : MonoBehaviour
{
    public void backButton()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}