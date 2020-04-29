using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MellowMadness.Core;

public class MainMenuController : MonoBehaviour {

    [Header("Menu Panels Array")]
    [SerializeField] private GameObject[] menuPanels;

    #region Event Handlers

    public void CallbackStartNewGame() {
        SceneController.Instance.LoadGameScene();
    }

    public void CallbackQuitGame() {
        SceneController.Instance.QuitGame();
    }

    public void CallbackOpenMenu (int menuIndex) {
        if (menuIndex < menuPanels.Length) {
            foreach (GameObject menuPanel in menuPanels) {
                menuPanel.SetActive(false);
            }

            menuPanels[menuIndex].SetActive(true);
        }
    }

    #endregion

}