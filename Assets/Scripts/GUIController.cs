using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

    public GameController gc;

    public Text debugTxt;

    public Toggle mapToggle;
    public GameObject exploringArrows, playerFunctions;

    public bool uiDisabled = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

    private void ToggleButtons(GameObject parent, params string[] leaveOn)
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
        gc.player.spRenderer.flipX = true;
    }
    public void MOVPlayerRight()
    {
        gc.currentBattle.MovRight();
        gc.currentBattle.AttackPreview(gc.player);
        gc.player.spRenderer.flipX = false;
    }
}
