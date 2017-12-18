using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat : GameHandler
{
    public enum BattleState { start, enemyTurn, playerTurn, win, loss };
    public static BattleState currentState;

    //List of Battles
    public static List<List<int>> listOfBattles = new List<List<int>>
    {
        new List<int> { 0, 0, 0 }
    };

    //Current Data
    public static List<Unit> currentBattle = new List<Unit>();
    public static List<float> fieldLocations = new List<float>();

    public static int currentTurn = -1;
    public static int currentPreviewTarget = 0;

    public static Attack currentAttack = null;
    public static Item currentItem = null;

    //Types
    public delegate void Effect(int index, float dmg);

    //List of Attack Effects
    public static Effect hurt = (int index, float dmg) =>
    {
        AI.hit(index, dmg);
    };

    public static Effect fireballEffect = (int index, float dmg) =>
    {
        AI.hit(index, dmg);
    };

    public static Effect slime = (int index, float dmg) =>
    {
        attackWith(new Attack("slime", null, hurt, 1, 1), currentTurn, currentBattle[currentTurn].magAtk);
    };

    //List of Attacks
    public static Attack attack = new Attack("attack", null, hurt, 0, true);

    public static Attack fireball = new Attack("fireball", null, fireballEffect, 1);

    public static Attack monsterAttack = new Attack("monsterAttack", "attack", hurt, 0, 0);
    public static Attack slimeAttack = new Attack("slimeAttack", "slime", slime, 0, 0);

    //List of Status Effects

    //List of Status Effect Effects

    //Tools
    public static void attackWith(Attack attack, int target)
    {
        attackWith(attack, target, currentBattle[currentTurn].atk);
    }
    public static void attackWith(Attack attack, int target, float dmg)
    {
        attack.dmg = dmg;
        for (int i = target - attack.range; i <= target + attack.range; i++)
        {
            if (i >= 0 && i < currentBattle.Count
                && (i <= target - attack.deadRange || i >= target + attack.deadRange))
            {
                attack.effect(i, attack.dmg);
                newPotionEffect(currentBattle[i]);
            }
        }
    }

    public static void attackPreview()
    {
        attackPreview(currentAttack);
    }

    public static void attackPreview(Attack attack)
    {
        attackPreview(attack, currentPreviewTarget);
    }

    public static void attackPreview(Attack attack, int target)
    {
        foreach(Transform child in getAttackPreviews().transform)
        {
            Destroy(child.gameObject);
        }

        currentPreviewTarget = target;

        if (attack != null)
        {
            for (int i = 0; i < currentBattle.Count; i++)
            {
                if ((attack.isMelee && Mathf.Abs(i - currentTurn) == 1) || 
                    (!attack.isMelee && (i >= target - attack.range && i <= target + attack.range) && (i <= target - attack.deadRange || i >= target + attack.deadRange)))
                {
                    GameObject preview = Instantiate(
                        gameHandler.attackView,
                        new Vector3(fieldLocations[i], floorY),
                        new Quaternion(),
                        gameHandler.attackPreviews.transform);
                    preview.transform.position -= new Vector3(0, 0.7f, 0);
                    preview.name = "attack";
                }
                else
                {
                    GameObject preview = Instantiate(
                        gameHandler.noAttackView,
                        new Vector3(fieldLocations[i], floorY),
                        new Quaternion(),
                        gameHandler.attackPreviews.transform);
                    preview.transform.position -= new Vector3(0, 0.7f, 0);
                    preview.name = "noAttack";

                }
            }
        }
    }

    public static void applyStatusEffect(int target, StatusEffect statusEffect)
    {
        statusEffect.dmg = getCurrentUnit().magAtk;
        currentBattle[target].statusEffects.Add(new StatusEffect(statusEffect));
    }
    
    public static void setCurrentAttack(Item item)
    {
        currentItem = item;
        setCurrentAttack(item.effect);
    }
    public static void setCurrentAttack(Attack attack)
    {
        currentAttack = attack;
        attackPreview(attack);
    }

    public static void nullCurrentAttack()
    {
        print("Nulled");
        currentAttack = null;
        currentItem = null;
        attackPreview(currentAttack);
    }

    //Move Functions
    public static void moveLeft()
    {
        currentBattle.Insert(currentTurn - 1, currentBattle[currentTurn]);
        currentBattle.RemoveAt(currentTurn + 1);
        currentTurn = currentTurn - 1;
        currentBattle[currentTurn].mov -= 1;
    }

    public static void moveRight()
    {
        currentBattle.Insert(currentTurn + 2, currentBattle[currentTurn]);
        currentBattle.RemoveAt(currentTurn);
        currentTurn = currentTurn + 1;
        currentBattle[currentTurn].mov -= 1;
    }

    public static bool isAdjacent(Unit unit)
    {
        if ((currentBattle.Count > currentTurn + 1 && currentBattle[currentTurn + 1].id == unit.id)
            || (currentTurn - 1 >= 0 && currentBattle[currentTurn - 1].id == unit.id)) return true;
        else return false;
    }

    //BattleTools
    public static void battleTrigger(List<int> newBattle)
    {
        if (gameState == GameState.inField)
        {
            gameState = GameState.inBattle;
            currentState = BattleState.start;

            foreach (int index in newBattle)
            {
                Instantiate(getListOfUnits()[index], getBattleUnits().transform);
            }

            float maxFieldWidth = fieldWidth * newBattle.Count + 1;
            float zero = maxFieldWidth / 2 * -1;
            for (int i = 0; i < newBattle.Count + 1; i++)
            {
                fieldLocations.Add(zero + fieldWidth * i);
            }

            gameHandler.StartCoroutine(gameHandler.waitThenAdvance());
        }
    }

    public delegate void bAHandler(); public static event bAHandler raiseBAHandlerEvent;

    public static void battleAdvance()
    {
        //turn off player ui
        GUIHandler.uiState = GUIHandler.UIState.disabled;
        currentAttack = null;
        getPlayerUnit().startPos = -1;

        //run end status effects
        if (currentTurn != -1)
        {
            currentBattle[currentTurn].gone = true;
            currentBattle[currentTurn].mov = currentBattle[currentTurn].maxMov;

            foreach (StatusEffect statusEffect in getCurrentUnit().statusEffects)
            {
                if (statusEffect.endEffect != null) {
                    statusEffect.endEffect(currentTurn, statusEffect.dmg);
                }
            }
        }

        //lose battle
        if (gameHandler.companion != null && getCompanionUnit().isDead && getPlayerUnit().isDead
            || gameHandler.companion == null && getPlayerUnit().isDead)
        {
            battleLose(); return;
        }

        //win battle
        bool gameIsWon = true;
        if (currentTurn != -1)
        {
            foreach (Unit unit in currentBattle)
            {
                if (unit.gameObject.tag != "Player" && unit.gameObject.tag != "Companion" && !unit.isDead)
                {
                    gameIsWon = false;
                }
            }
            if (gameIsWon)
            {
                battleWin();
                return;
            }
        }

        //pick next turn
        currentState = BattleState.enemyTurn;

        int greatestSpd = -1, newTurn = 0;
        bool restart = true;

        for (int i = currentBattle.Count - 1; i >= 0; i--)
        {
            Unit unit = currentBattle[i];
            if (!unit.gone && !unit.isDead && unit.spd >= greatestSpd)
            {
                greatestSpd = (int)unit.spd;
                restart = false;
                newTurn = i;
            }
        }

        if (restart)
        {
            foreach (Unit unit in currentBattle)
            {
                unit.gone = false;
            }
            Debug.Log("Restart...");
            currentTurn = -1;
            battleAdvance();
            return;
        }

        currentTurn = newTurn;
        gameHandler.StartCoroutine(gameHandler.waitThenGo());

        raiseBAHandlerEvent();

        //run start status effects
        if (currentTurn != -1)
        {
            foreach (StatusEffect statusEffect in getCurrentUnit().statusEffects)
            {
                statusEffect.timeLeft -= 1;

                if (statusEffect.startEffect != null)
                {
                    statusEffect.startEffect(currentTurn, statusEffect.dmg);
                }
                if (statusEffect.timeLeft <= 0)
                {
                    getCurrentUnit().statusEffects.Remove(statusEffect);
                    break;
                }
            }
        }
    }

    public static void battleWin()
    {
        currentState = BattleState.win;
        gameState = GameState.inField;
        GUIHandler.uiState = GUIHandler.UIState.exploring;
        fieldLocations = new List<float>();
        foreach (Unit unit in currentBattle)
        {
            if (unit.gameObject.tag != "Player" && unit.gameObject.tag != "Companion")
            {
                Destroy(unit.gameObject);
                currentBattle.Remove(unit);
                battleWin();
                break;
            }
        }
        gameHandler.gameWonText.SetActive(true);
        currentTurn = -1;
        var currentTile = Exploring.currentMap.map[(int)Exploring.coords.y][(int)Exploring.coords.x];
        currentTile.func = Exploring.tileNormal;
    }

    public static void battleLose()
    {
        gameHandler.gameOverText.SetActive(true);
        currentState = BattleState.loss;
    }
}
