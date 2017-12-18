using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {

    public bool cheatInfiniteItems, cheatInvincible;

    public Text debugTxt;
    
    public enum GameState { inField, inBattle }
    public static GameState gameState = GameState.inField;
    
    public GameObject 
        battleUnits, player, companion, gameOverText, gameWonText, 
        infoBoxPref, potionEffectPref, dmgValPref,
        attackPreviews, attackView, noAttackView;
    public List<GameObject> listOfUnits;
    
    public static GameHandler gameHandler;
    public static GameObject mainCanvas;

    public static int newID = 0;
    public static float fieldWidth = 3, floorY = 1.5f;

    public static Dictionary<string, Item> itemList = new Dictionary<string, Item>();
    public static List<List<Item>> inventory = new List<List<Item>>();
    public List<Texture2D> itemTexList;
    public List<Texture2D> tileTexList;

    private static Tile n, s, e, t, b;

    //CALLBACKS
    private void Awake()
    {
        gameHandler = this;
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
    }
    private void Start()
    {

        Exploring.moveToStart();

        //item types
        itemList.Add("testitem1", new Item("testitem1", 3, itemTexList[0], Combat.fireball));

        //test items
        give(itemList["testitem1"]);
}

    private void Update()
    {
        //print debug txt
        var v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        v3.x = Mathf.Round(v3.x * 100) / 100;
        v3.y = Mathf.Round(v3.y * 100) / 100;
        debugTxt.text = "FPS: " + Mathf.Round(1 / Time.deltaTime) + "\n"
            + "MousePos (x, y): (" + Input.mousePosition.x + ", " + (Screen.height - Input.mousePosition.y) + ")\n"
            + "WorldPos (x, y): (" + v3.x + ", " + v3.y + ")\n"
            + "Coordinates: " + Exploring.coords;
    }

    //Custom Functions
    public IEnumerator waitThenAdvance()
    {
        yield return 0;
        foreach(Unit unit in Combat.currentBattle)
        {
            if (unit.alliance != "friendly")
            {
                unit.facingLeft = true;
            }
        }
        Combat.battleAdvance();
    }

    public IEnumerator waitThenGo()
    {
        yield return new WaitForSeconds(1);
        if (gameState == GameState.inBattle) Combat.currentBattle[Combat.currentTurn].ai();
    }

    public static void newPotionEffect(Unit target)
    {
        GameObject pEff = Instantiate(gameHandler.potionEffectPref, new Vector3(Combat.fieldLocations[findUnit(target)], floorY), new Quaternion());
        pEff.name = "potionEffect";

        //set animation type based on potion effect: TODO
    }

    public static void give(Item item)
    {
        foreach (List<Item> stack in inventory)
        {
            if (stack[0].name == item.name && stack.Count < stack[0].stackSize)
            {
                stack.Add(item);
                return;
            }
        }
        inventory.Add(new List<Item> { new Item(item) });
        sortInv("alpha");
    }

    public static void take(Item item)
    {
        foreach (List<Item> stack in inventory)
        {
            if(stack[0].name == item.name)
            {
                stack.Remove(item);
                if (stack.Count == 0)
                {
                    inventory.Remove(stack);
                    return;
                }
            }
        }
    }

    public static void sortInv(string sortType)
    {
        if (sortType == "alpha" || sortType == "alphabetical")
        {
            inventory = inventory.OrderBy(x => x[0].name).ToList();
        }
    }

    //Getters & Setters

    public static int findUnit(Unit unit)
    {
        for (int i = Combat.currentBattle.Count - 1; i >= 0; i--)
        {
            if (Combat.currentBattle[i].id == unit.id) return i;
        }
        return -1;
    }

    public static List<GameObject> getListOfUnits() { return gameHandler.listOfUnits; }

    public static GameObject getBattleUnits() { return gameHandler.battleUnits; }

    public static Unit getCurrentUnit() { return Combat.currentBattle[Combat.currentTurn]; }

    public static GameObject getAttackPreviews() { return gameHandler.attackPreviews; }

    public static GameObject getPlayer() { return gameHandler.player; }
    public static Unit getPlayerUnit() { return gameHandler.player.GetComponent<UnitHandler>().unit; }
    public static GameObject getCompanion() { return gameHandler.companion; }
    public static Unit getCompanionUnit() { return gameHandler.companion.GetComponent<UnitHandler>().unit; }
}
