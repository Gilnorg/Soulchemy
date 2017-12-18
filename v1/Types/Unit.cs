using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit {

    public string name, alliance;
    public System.Action ai;

    public List<Attack> moveList = new List<Attack>();

    public int id;
    public float 
        basehp, maxhp, hp,
        baseAtk, atk,
        baseMagAtk, magAtk,
        baseDef, def,
        baseSpd, spd,
        startPos = -1;
    public int baseMov, maxMov, mov;
    public bool gone, isDead = false;
    public GameObject gameObject;
    public bool facingLeft = false;
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    public Unit(GameObject newHost, string newName, string newAlliance, string newAi, int newHp, int newAtk, int newMagAtk, int newDef, int newSpd, int newMove)
    {
        setValues(newHost, newName, newAlliance, newAi, newHp, newAtk, newMagAtk, newDef, newSpd, newMove, null);
    }

    public Unit(GameObject newHost, string newName, string newAlliance, string newAi, int newHp, int newAtk, int newMagAtk, int newDef, int newSpd, int newMove, List<Attack> newMoveList)
    {
        setValues(newHost, newName, newAlliance, newAi, newHp, newAtk, newMagAtk, newDef, newSpd, newMove, newMoveList);
    }

    private void setValues(GameObject newHost, string newName, string newAlliance, string newAi, int newHp, int newAtk, int newMagAtk, int newDef, int newSpd, int newMove, List<Attack> newMoveList)
    {
        id = GameHandler.newID;
        GameHandler.newID += 1;
        gone = false;

        gameObject = newHost;
        name = newName;
        if (newAlliance != "friendly" && newAlliance != "enemy")
        {
            Debug.LogError("Incorrect Alliance. Got " + newAlliance + " instead of friendly or enemy.");
            alliance = "enemy";
        }
        else alliance = newAlliance;

        if (AI.dict.ContainsKey(newAi)) ai = AI.dict[newAi];
        else ai = AI.dict["default"];

        basehp = maxhp = hp = newHp;

        baseAtk = atk = newAtk;

        baseMagAtk = magAtk = newMagAtk;

        baseDef = def = newDef;

        baseSpd = spd = newSpd;

        baseMov = maxMov = mov = newMove;

        moveList = newMoveList;

    }

}
