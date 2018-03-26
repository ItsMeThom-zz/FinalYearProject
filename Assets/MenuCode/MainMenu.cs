using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    GameController Controller = GameController.GetSharedInstance();

    [SerializeField]
    public TMP_InputField nameInput;

    public void MainPlay()
    {

        //load name entry menu canvas
        //load scene async
        // when secne load completes
        // find() WorldReady object in scene
        // if found:
        // set scene active.
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void SetBaseSeed()
    {
        string seedstring = nameInput.text;
        int result = Utils.StringUtils.StringToUnicode(seedstring);
        Debug.Log(seedstring + " = " + result);
        Controller.BaseSeed = result * result;
        Debug.Log("Seed: " + result * result);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
