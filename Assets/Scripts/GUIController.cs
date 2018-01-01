using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

    private GameController gc;

    public Text debugTxt;

    public Toggle mapToggle;
    public GameObject exploringArrows, playerFunctions;

    public bool uiDisabled = false;

    private void Start()
    {
        gc = GameController.main;
    }

    private void OnGUI()
    {
        if (gc.state == GameState.inField)
        {
            if (!uiDisabled)
            {
                exploringArrows.SetActive(true);
                playerFunctions.SetActive(false);
            }
            else
            {
                exploringArrows.SetActive(false);
                playerFunctions.SetActive(false);
            }
        }
        else if (gc.state == GameState.inBattle)
        {
            if (gc.currentBattle.state == BattleState.playerTurn)
            {
                exploringArrows.SetActive(false);
                playerFunctions.SetActive(true);
            }
            else
            {
                exploringArrows.SetActive(false);
                playerFunctions.SetActive(false);
            }
        }
    }

    public void ToggleButtons(GameObject parent, string[] leaveOn)
    {
        foreach (Transform button in parent.transform)
        {
            bool keepIt = false;

            foreach (string name in leaveOn)
            {
                if (button.name == name)
                {
                    keepIt = true;
                    break;
                }
            }

            if (!keepIt)
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    //BUTTON TRIGGERS
    public void MOVPlayerLeft()
    {
        gc.currentBattle.MovLeft();
        gc.currentBattle.AttackPreview(gc.player);
    }
    public void MOVPlayerRight()
    {
        gc.currentBattle.MovRight();
        gc.currentBattle.AttackPreview(gc.player);
    }
}
