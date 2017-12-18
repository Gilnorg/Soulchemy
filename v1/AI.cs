using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Combat {

    //AI FUNCTIONS
    public static Dictionary<string, Action> dict = new Dictionary<string, Action>
    {
        { "default", DefaultAi },
        { "player", PlayerAi },
        { "slime", SlimeAI }
    };

    public static Dictionary<string, List<Attack>> allMoveLists = new Dictionary<string, List<Attack>>
    {
        { "slime", new List<Attack> {
            new Attack (monsterAttack, 75),
            new Attack (slimeAttack, 65)
        } }
    };

    public static Attack tempAttack = null;
    public static int tempTarget = -1;

    public static void DefaultAi()
    {
        Debug.LogError( getCurrentUnit().name + getCurrentUnit().id + " didn't get a proper AI.");
        battleAdvance();
    }

    public static void PlayerAi()
    {
        Debug.Log("Player attacks!");
        currentState = BattleState.playerTurn;
        getPlayerUnit().startPos = currentTurn;
        GUIHandler.uiState = GUIHandler.UIState.action;
    }

    public static void SlimeAI()
    {
        Debug.Log(getCurrentUnit().name + getCurrentUnit().id + " attacks!");

        //decide target
        Unit target = null; Unit attacker = currentBattle[currentTurn];
        bool shouldAttack = true;

        if (getCompanion() != null && getPlayerUnit().hp > getCompanionUnit().hp)
        {
            if (Mathf.Abs(findUnit(getCompanionUnit()) - currentTurn) <= attacker.maxMov)
            {
                target = getCompanionUnit();
            }
        }
        else
        {
            if (Mathf.Abs(findUnit(getPlayerUnit()) - currentTurn) <= attacker.maxMov)
            {
                target = getPlayerUnit();
            }
        }

        if (target == null)
        {
            if (getCompanion() != null && UnityEngine.Random.Range(1, 100) > 50)
            {
                target = getCompanionUnit();
            }
            else
            {
                target = getPlayerUnit();
            }
            shouldAttack = false;
        }

        //move to target
        attacker.facingLeft = faceTarget(target);

        while (attacker.mov > 0 && !isAdjacent(target))
        {
            if (findUnit(target) < currentTurn)
            {
                moveLeft();
            }
            else
            {
                moveRight();
            }
        }

        //decide attack
        if (shouldAttack)
        {
            selectAttack(attacker, target);
        }

        attacker.gameObject.GetComponent<Animator>().SetTrigger(tempAttack.visEffect);
    }

    public static void waitThenAttack()
    {
        //perform attack
        if (tempAttack != null)
        {
            attackWith(tempAttack, tempTarget);

            tempAttack = null;
            tempTarget = -1;
        }
    }

    public static void selectAttack(Unit attacker, Unit target)
    {
        Attack chosenAttack = attacker.moveList[0];
        int chance = UnityEngine.Random.Range(1, 100);

        foreach (Attack candidate in attacker.moveList)
        {
            if (candidate.priority >= chance)
            {
                chosenAttack = candidate;
            }
        }

        tempAttack = chosenAttack;
        tempTarget = findUnit(target);
    }

    public static bool faceTarget(Unit target)
    {
        return findUnit(target) < currentTurn;
    }

    //ATTACKS
    public static void hit (int index)
    {
        Unit target = currentBattle[index];
        Unit attacker = currentBattle[currentTurn];
        var atk = currentBattle[currentTurn].atk;

        target.hp -= atk;
        if (target.hp <= 0) { target.hp = 0; target.isDead = true; }
        Debug.Log(attacker.name + attacker.id + " hits " + target.name + target.id + " for " + atk + " damage");
    }

    public static void hit(int index, float atk)
    {
        Unit target = currentBattle[index];

        target.hp -= atk;
        Debug.Log(target.name + target.id + " takes " + atk + " damage");
        if (target.hp <= 0) { target.hp = 0; target.isDead = true; Debug.Log(target.name + " is dead!"); }
    }
}
