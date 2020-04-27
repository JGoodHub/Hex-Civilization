using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MellowMadness.Core;

public class MainMenuController : MonoBehaviour {
    
    #region Event Handlers

    public void ButtonStartNewGame() {
        SceneController.Instance.LoadGameScene();
    }

    public void ButtonOpenSettings () {
        //Open the settings panel
    }

    public void ButtonQuitGame () {
        SceneController.Instance.QuitGame();
    }

    #endregion

}