using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHandler : MonoBehaviour {

    public string unitName, alliance, ai, moveList;
    public int hp, atk, magAtk, def, spd, mov;
    public float unitFloorY;

    public bool mouseOver = false;
    private Vector3 baseScale;

    private float lastHoverTime;

    public Unit unit;
    private SpriteRenderer spRend;
    
	void Start () {
        baseScale = gameObject.transform.localScale;
        spRend = GetComponent<SpriteRenderer>();

        if (AI.allMoveLists.ContainsKey(moveList))
        {
            unit = new Unit(gameObject, unitName, alliance, ai, hp, atk, magAtk, def, spd, mov, AI.allMoveLists[moveList]);
        }
        else
        {
            unit = new Unit(gameObject, unitName, alliance, ai, hp, atk, magAtk, def, spd, mov);
        }
        Combat.currentBattle.Add(unit);

        transform.position = new Vector3(0, GameHandler.floorY + transform.lossyScale.y / 2, 0);

        Combat.raiseBAHandlerEvent += handleBattleAdvance;
        GUIHandler.onAttack += handleOnAttack;
	}
	
	void Update () {
        //move to field position
        Vector3 targetPos;

        if (GameHandler.gameState == GameHandler.GameState.inBattle)
        {
            targetPos = new Vector3(Combat.fieldLocations[GameHandler.findUnit(unit)], GameHandler.floorY + unitFloorY);
        }
        else targetPos = new Vector3(0, GameHandler.floorY + unitFloorY);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            (Vector3.Distance(transform.position, targetPos) * 8 + 0.1f) * Time.deltaTime);

        spRend.flipX = unit.facingLeft;
        //grow/shrink
        if (lastHoverTime + 0.1f < Time.timeSinceLevelLoad) mouseOver = false;

        if (mouseOver)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, baseScale * 1.1f, Time.deltaTime * 2);
        }
        else if (!mouseOver)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, baseScale, Time.deltaTime * 2);
        }
    }

    private void OnMouseOver()
    {
        if (Combat.currentState == Combat.BattleState.playerTurn)
        {
            //set mouseOver to true
            mouseOver = true;
            Combat.attackPreview(Combat.currentAttack, GameHandler.findUnit(unit));
            lastHoverTime = Time.timeSinceLevelLoad;
        }
    }

    public void handleOnAttack()
    {
        if (mouseOver && Combat.currentAttack != null)
        {
            if (Combat.currentAttack.isMelee && !Combat.isAdjacent(unit))
            {
                print("Out of range");
                return;
            }
            else
            {
                Combat.attackWith(Combat.currentAttack, GameHandler.findUnit(unit));
            }
        }
    }

    public void waitThenAttack()
    {
        AI.waitThenAttack();
    }

    public void waitThenAdvance()
    {
        Combat.battleAdvance();
    }

    private void handleBattleAdvance()
    {
        mouseOver = false;
    }
}
