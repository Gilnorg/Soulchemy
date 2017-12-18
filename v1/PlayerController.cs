using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Unit playerUnit;

	// Use this for initialization
	void Start () {
        playerUnit = GameHandler.getPlayerUnit();
	}
	
	// Update is called once per frame
	void Update () {
        if (playerUnit == null) { playerUnit = GameHandler.getPlayerUnit(); }

        if (Combat.currentAttack != null && Combat.currentPreviewTarget < Combat.currentBattle.Count && Combat.currentPreviewTarget >= 0)
        {
            GameHandler.getPlayerUnit().facingLeft = AI.faceTarget(Combat.currentBattle[Combat.currentPreviewTarget]);
        }

        //cheats
        if (playerUnit != null)
        {
            if (GameHandler.gameHandler.cheatInvincible)
            {
                playerUnit.hp = playerUnit.maxhp;
            }
        }
    }
}
