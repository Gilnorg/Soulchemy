using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIHandler : MonoBehaviour {
    
    public enum UIState { exploring, conversing, action, attacking, inventory, disabled }
    public static UIState uiState = UIState.exploring;

    public List<AudioSource> audioSources;

    private GUIStyle emptyStyle = new GUIStyle();

    public GUIStyle healthBarStyle;
    public Rect healthBarRect;

    public GUIStyle MOVPipsStyle;
    public Rect MOVPipsRect;
    public float MOVPipsMargins;

    public Texture2D leftMoveTex;
    public Rect leftMoveRect;

    public Texture2D rightMoveTex;
    public Rect rightMoveRect;

    public GUIStyle exploreStyle;

    public Texture2D playerMarker;
    public float mapWidth;
    public float mapIconMargins;
    public Rect mapIconRect;
    private bool showMap = true;

    public Texture2D leaveTex;
    public Rect leaveRect;

    public Texture2D upArrowTex;
    public Rect upArrowRect;

    public Texture2D downArrowTex;
    public Rect downArrowRect;

    public Texture2D leftArrowTex;
    public Rect leftArrowRect;

    public Texture2D rightArrowTex;
    public Rect rightArrowRect;

    public Texture2D dialogueStartTex;
    public Rect dialogueStartRect;

    public Texture2D dialogueContinueTex;
    public Rect dialogueContinueRect;

    public Rect dialogueBoxRect;

    public GUIStyle actionStyle;

    public Texture2D attackTex;
    public Rect attackRect;

    public Texture2D invTex;
    public Rect invRect, invButtonRect;

    public float invButtonMargin;

    public int invWidth, invHeight;
    public Rect invLoc;

    public float clickSens;
    private float lastClickTime = -1, holdTime = 0;
    private bool isDragging = false, shouldRepeat = false;

    public delegate void AttackHandler();
    public static event AttackHandler onAttack;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Combat.currentAttack != null)
        {
            //checks if mouse is over a unit
            bool mouseOver = false;
            foreach(Unit unit in Combat.currentBattle)
            {
                //if attack is melee, check if target is within melee range
                //otherwise, if mouse is over target, attack
                if (!(Combat.currentAttack.isMelee && Mathf.Abs(Combat.currentTurn - GameHandler.findUnit(unit)) != 1) 
                    && unit.gameObject.GetComponent<UnitHandler>().mouseOver)
                {
                    mouseOver = true;
                    break;
                }
            }

            if (mouseOver && Combat.currentAttack != null)
            {
                raiseOnAttack();

                if (Combat.currentItem != null && !GameHandler.gameHandler.cheatInfiniteItems) { GameHandler.take(Combat.currentItem); }
                Combat.nullCurrentAttack();
                Combat.battleAdvance();
            }
            else if (uiState != UIState.attacking)
            {
                print(leftMoveRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) + ", " + rightMoveRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)));
                Combat.nullCurrentAttack();
            }
        }

        if (isDragging && !Input.GetMouseButton(0))
        {
            bool mouseOver = false;
            foreach (Unit unit in Combat.currentBattle)
            {
                if (unit.gameObject.GetComponent<UnitHandler>().mouseOver)
                {
                    mouseOver = true;
                    break;
                }
            }

            if (mouseOver && Combat.currentAttack != null)
            {
                raiseOnAttack();

                isDragging = false;
                holdTime = 0;

                if (Combat.currentItem != null && !GameHandler.gameHandler.cheatInfiniteItems) { GameHandler.take(Combat.currentItem); }
                Combat.nullCurrentAttack();
                Combat.battleAdvance();
            }
            else
            {
                isDragging = false;
                holdTime = 0;
                Combat.nullCurrentAttack();
            }
        }
    }

    protected virtual void raiseOnAttack()
    {
        AttackHandler handler = onAttack;

        if (handler != null)
        {
            handler();
        }
    }

    private void OnGUI()
    {
        foreach (Unit unit in Combat.currentBattle)
        {
            //draw health bars
            var newRect = healthBarRect;
            newRect.x = Camera.main.WorldToScreenPoint(unit.gameObject.transform.position).x - healthBarRect.x;
            GUI.Box(newRect, "");

            var newRect2 = newRect;
            newRect2.width *= unit.hp / unit.maxhp;
            GUI.Box(newRect2, "HP: " + unit.hp, healthBarStyle);

            //draw MOV pips
            for (int i = unit.mov; i > 0; i--)
            {
                var tempMOVpipsRect = MOVPipsRect;
                tempMOVpipsRect.x = newRect2.x + MOVPipsRect.x + i * (MOVPipsRect.width + MOVPipsMargins);
                tempMOVpipsRect.y = healthBarRect.y + MOVPipsRect.y;
                GUI.Box(tempMOVpipsRect, "", MOVPipsStyle);
            }
        }

        //draw movement arrows
        if (GameHandler.gameState == GameHandler.GameState.inBattle && uiState != UIState.disabled)
        {
            //draw moveLeft arrow at 370, 315, 75, 75
            if (GUI.Button(leftMoveRect, new GUIContent(leftMoveTex), emptyStyle))
            {
                if(Combat.currentTurn >= 1) playerMoveLeft();
                audioSources[0].Play();
            }

            //draw moveRight arrow at 550, 315, 75, 75
            if (GUI.Button(rightMoveRect, new GUIContent(rightMoveTex), emptyStyle))
            {
                playerMoveRight();
                audioSources[0].Play();
            }
        }

        //draw exploration arrows
        if (uiState == UIState.exploring)
        {
            //draw up arrow
            if (Exploring.adjecentToTile("up") && GUI.Button(upArrowRect, new GUIContent(upArrowTex, "Move Up"), emptyStyle))
            {
                travelUp();
                audioSources[0].Play();
            }
            //draw down arrow
            if (Exploring.adjecentToTile("down") && GUI.Button(downArrowRect, new GUIContent(downArrowTex, "Move Down"), emptyStyle))
            {
                travelDown();
                audioSources[0].Play();
            }
            //draw left arrow
            if (Exploring.adjecentToTile("left") && GUI.Button(leftArrowRect, new GUIContent(leftArrowTex, "Move Left"), emptyStyle))
            {
                travelLeft();
                audioSources[0].Play();
            }
            //draw right arrow
            if (Exploring.adjecentToTile("right") && GUI.Button(rightArrowRect, new GUIContent(rightArrowTex, "Move Right"), emptyStyle))
            {
                travelRight();
                audioSources[0].Play();
            }

            //draw map
            showMap = GUI.Toggle(new Rect(mapIconRect.x + 20, mapIconRect.y - 20, 40, 40), showMap, new GUIContent());
            if (showMap)
            {
                drawMap();
            }

            //draw exit button
            var mapString = Exploring.getCurrentTile().mapString;
            if (mapString != null)
            {
                if (GUI.Button(leaveRect, new GUIContent("Leave", leaveTex)))
                {
                    Exploring.embark(Exploring.getMap(mapString));
                }
            }

            //draw dialogue button
            var currentDialogueString = Exploring.getCurrentTile().dialogue;
            if (currentDialogueString != null)
            {
                if (GUI.Button(dialogueStartRect, new GUIContent("Talk to " + DialogueHandler.findDialogue(currentDialogueString).characterName, dialogueStartTex)))
                {
                    uiState = UIState.conversing;
                    DialogueHandler.startConversation(currentDialogueString);
                }
            }
        }

        //draw dialogue box
        else if (uiState == UIState.conversing)
        {
            GUI.Box(dialogueBoxRect, new GUIContent(DialogueHandler.currentSentence));

            if (DialogueHandler.currentFullSentence.choices.Count < 1)
            {
                if (GUI.Button(dialogueContinueRect, new GUIContent("Next", dialogueContinueTex)))
                {
                    DialogueHandler.displayNextSentence();
                }
            }
            else
            {
                Rect tempRect = dialogueContinueRect;
                var choices = DialogueHandler.currentFullSentence.choices;

                for (int i = 0; i < choices.Count; i++)
                {
                    string choice = choices[i];
                    tempRect.x = dialogueBoxRect.x - dialogueContinueRect.width / 2 + dialogueBoxRect.width / (choices.Count + 1) * (i + 1);

                    if (GUI.Button(tempRect, new GUIContent(DialogueHandler.findDialogue(choice).name)))
                    {
                        DialogueHandler.startConversation(choice);
                    }
                }
            }
        }

        //draw combat actions
        else if (uiState == UIState.action)
        {

            //draw attack button at 300, 400, 200, 40
            if (GUI.Button(attackRect, new GUIContent("Attack", attackTex), actionStyle))
            {
                Combat.setCurrentAttack(Combat.attack);
                uiState = UIState.attacking;
            }

            //draw inventory button at 300, 450, 200, 40
            if (GUI.Button(invRect, new GUIContent("Inventory", invTex), actionStyle))
            {
                uiState = UIState.inventory;
            }
        }

        //draw attack targets
        else if (uiState == UIState.attacking)
        {
            //draw attack button at 300, 400, 200, 40
            if (GUI.Button(attackRect, new GUIContent("Attack", attackTex), actionStyle))
            {
                Combat.nullCurrentAttack();
                uiState = UIState.action;
            }

        }

        //draw inventory
        else if (uiState == UIState.inventory)
        {
            //draw inventory button at 300, 450, 200, 40
            if (GUI.Button(attackRect, new GUIContent("Inventory", invTex), actionStyle))
            {
                uiState = UIState.action;
            }

            drawInv();

            if (Combat.currentItem != null)
            {
                GUI.Label(new Rect(Event.current.mousePosition, new Vector2(50, 50)), new GUIContent(Combat.currentItem.tex));
            }
        }

        //draw Tooltip
        GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 400, 10), GUI.tooltip, emptyStyle);

        //what the fuck unity (works while true)
        if (shouldRepeat) shouldRepeat = false; else shouldRepeat = true;
    }

    public void drawInv()
    {
        GUI.Box(new Rect(invLoc.x, invLoc.y, invLoc.width, invLoc.height), "");
        for (int row = 0; row < invHeight; row++)
        {
            for (int col = 0; col < invWidth; col++)
            {
                var index = row * invWidth + col;
                if (index < GameHandler.inventory.Count)
                {
                    //draw item buttons
                    if (shouldRepeat)
                    {
                        Rect buttonRect = new Rect(
                            invLoc.x + invButtonRect.x + (invButtonRect.width + invButtonMargin) * col,
                            invLoc.y + invButtonRect.y + (invButtonRect.height + invButtonMargin) * row,
                            invButtonRect.width, invButtonRect.height
                            );
                        if (GUI.RepeatButton(buttonRect, new GUIContent(GameHandler.inventory[index][0].tex)) && !isDragging)
                        {
                            holdTime += Time.deltaTime;
                            if (holdTime > clickSens)
                            {
                                isDragging = true;
                                Combat.setCurrentAttack(GameHandler.inventory[index][0]);
                            }
                        }
                        else if (!isDragging)
                        {
                            if (buttonRect.Contains(Event.current.mousePosition) && holdTime > 0 && holdTime < clickSens)
                            {
                                //double click
                                if (Time.timeSinceLevelLoad - lastClickTime <= clickSens)
                                {
                                    if (Combat.currentItem != GameHandler.inventory[index][0])
                                    {
                                        Combat.setCurrentAttack(GameHandler.inventory[index][0]);
                                    }
                                    else
                                    {
                                        Combat.nullCurrentAttack();
                                    }
                                    lastClickTime = -1;
                               }
                               //one click
                               else
                               {
                                    lastClickTime = Time.timeSinceLevelLoad;
                               }

                            }

                            holdTime = 0;
                        }
                    }
                    GUI.Label(new Rect(invLoc.x + 10 + 50 * col, invLoc.y + 10 + 45 * row, 40, 20), GameHandler.inventory[index].Count.ToString());
                }
                else
                {
                    GUI.Button(new Rect(invLoc.x + invButtonRect.x + (invButtonRect.width + invButtonMargin) * col,
                        invLoc.y + invButtonRect.y + (invButtonRect.height + invButtonMargin) * row, invButtonRect.width, invButtonRect.height), "");
                }
                
            }
        }

    }

    public void drawMap()
    {
        GUI.Box(new Rect(mapIconRect.x - mapWidth, mapIconRect.y, mapWidth, mapWidth), new GUIContent());

        Rect tempMapRect = new Rect();
        float tempWidth;
        if (Exploring.currentMap.map.Count > Exploring.currentMap.map[0].Count)
        {
            tempWidth = mapWidth / Exploring.currentMap.map.Count;
        }
        else
        {
            tempWidth = mapWidth / Exploring.currentMap.map[0].Count;
        }

        tempMapRect.height = tempWidth;
        tempMapRect.width = tempWidth;

        for (int y = Exploring.currentMap.map.Count - 1; y >= 0; y--)
        {
            for (int x = Exploring.currentMap.map[y].Count - 1; x >= 0; x--)
            {
                tempMapRect.x = mapIconRect.x - (Exploring.currentMap.map[y].Count - x) * (tempMapRect.width);
                tempMapRect.y = mapIconRect.y + y * (tempMapRect.height);
                GUI.Box(tempMapRect, new GUIContent(Exploring.currentMap.map[y][x].tex), emptyStyle);
            }
        }
        tempMapRect.x = mapIconRect.x - (Exploring.currentMap.map[0].Count - Exploring.coords.x) * (tempMapRect.width);
        tempMapRect.y = mapIconRect.y + Exploring.coords.y * (tempMapRect.height);
        GUI.Box(tempMapRect, new GUIContent(playerMarker), emptyStyle);
    }

    //Button Triggers
    public void fakeTrigger()
    {
        if (GameHandler.gameState == GameHandler.GameState.inField && Exploring.currentMap.isCheckpoint)
        {
            Exploring.embark(Exploring.testMap);
        }
        else if (GameHandler.gameState == GameHandler.GameState.inField)
        {
            Combat.battleTrigger(Combat.listOfBattles[Mathf.FloorToInt(Random.Range(0, Combat.listOfBattles.Count - 0.1f))]);
        }
        else Combat.battleWin();
    }

    public void skipTurn()
    {
        Combat.battleAdvance();
    }

    public void travelUp()
    {
        if (Exploring.coords.y - 1 >= 0
            && Exploring.currentMap.map[(int)Exploring.coords.y - 1][(int)Exploring.coords.x].type != "noTile")
        {
            Exploring.coords.y -= 1;
            Exploring.activateTile();
        }
    }
    public void travelDown()
    {
        if (Exploring.coords.y + 1 < Exploring.currentMap.map.Count
            && Exploring.currentMap.map[(int)Exploring.coords.y + 1][(int)Exploring.coords.x].type != "noTile")
        {
            Exploring.coords.y += 1;
            Exploring.activateTile();
        }
    }
    public void travelLeft()
    {
        if (Exploring.coords.x - 1 >= 0
            && Exploring.currentMap.map[(int)Exploring.coords.y][(int)Exploring.coords.x - 1].type != "noTile")
        {
            Exploring.coords.x -= 1;
            Exploring.activateTile();
        }
    }
    public void travelRight()
    {
        if (Exploring.coords.x + 1 < Exploring.currentMap.map[(int)Exploring.coords.y].Count
            && Exploring.currentMap.map[(int)Exploring.coords.y][(int)Exploring.coords.x + 1].type != "noTile")
        {
            Exploring.coords.x += 1;
            Exploring.activateTile();
        }
    }

    public void playerMoveLeft()
    {
        if (Mathf.Abs(GameHandler.getPlayerUnit().startPos - Combat.currentTurn) > Mathf.Abs(GameHandler.getPlayerUnit().startPos - Combat.currentTurn + 1))
        {
            Combat.moveLeft();
            GameHandler.getPlayerUnit().mov += 2;
        }
        else if (GameHandler.getPlayerUnit().mov > 0)
        {
            Combat.moveLeft();
        }

        GameHandler.getPlayerUnit().facingLeft = true;

        Combat.attackPreview();
    }

    public void playerMoveRight()
    {
        if (Mathf.Abs(GameHandler.getPlayerUnit().startPos - Combat.currentTurn) > Mathf.Abs(GameHandler.getPlayerUnit().startPos - Combat.currentTurn - 1))
        {
            Combat.moveRight();
            GameHandler.getPlayerUnit().mov += 2;
        }
        else if (GameHandler.getPlayerUnit().mov > 0)
        {
            Combat.moveRight();
        }

        GameHandler.getPlayerUnit().facingLeft = false;

        Combat.attackPreview();
    }
}
