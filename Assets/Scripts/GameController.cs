using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { inField, inBattle }

public delegate void Empty();

public class GameController : MonoBehaviour
{
    public static GameController main;

    public const float floorY = 1.5f;
    public const float unitWidth = 2.1f;

    public bool cheatInfiniteItems, cheatInvincible;

    public GameObject mainCanvas, battleUnits;

    public GUIController guiC;
    public MiniMapController mapHandler;
    public BigTileHandler bigTileHandler;

    public PCController player;
    public Entity companion;

    public float center;

    public GameObject attackReticle, noAttackReticle,
        healthBar,
        defaultVisEffect, blackOut;

    private Animator blackOutAnimator;
    
    [System.Serializable]
    public struct BattleType
    {
        public string name;
        public int chance;
        public List<GameObject> enemies;
    }

    public GameState state = GameState.inField;

    public Map currentMap;

    public Battle currentBattle;
    public PlayerAttack currentPlayerAttack
    {
        get { return currentBattle.currentPlayerAttack; }
        set { currentBattle.currentPlayerAttack = value; }
    }

    public List<BattleType> battles;

    public List<List<Item>> inventory = new List<List<Item>>(new List<Item>[30]);

    public List<Sprite> itemSprites = new List<Sprite>();
    public List<GameObject> setPiecePrefs = new List<GameObject>();

    public Item currentItem = null;

    private static int currentID = -1;
    public static int newID
    {
        get { currentID++; return currentID; }
    }


    // Use this for initialization
    private void Awake()
    {
        main = this;

        currentMap = new Map();

        blackOutAnimator = blackOut.GetComponent<Animator>();
    }

    void Start ()
    {
        GiveItem(new Item.Test(), 20);

	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    //Field Management
    public Empty fadeOutFunc;

    public void FadeOut(Empty newFadeOutFunc)
    {
        fadeOutFunc = newFadeOutFunc;
        blackOutAnimator.Play("FadeToBlackAnim");
    }

    //Battle Management
    public IEnumerator BattleTrigger(int battle = 0)
    {
        if (battle >= 0)
        {
            if (!currentBattle.set)
            {
                currentBattle = new Battle(battles[battle].enemies);
            }

            yield return 0;

            currentBattle.Trigger();
        }
    }

    public void SetCurrentAttack(PlayerAttack newAttack)
    {
        currentBattle.currentPlayerAttack = newAttack;
    }

    //Inv Management
    public event Empty RaiseInvEvent;

    public void UseItem()
    {

    }

    public void GiveItem(Item item, int amount = 1)
    {
        int startAmount = amount;
        bool gotItem = false;

        while (amount > 0)
        {
            for (int i = 0; i < inventory.Count && !gotItem && amount > 0; i++)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = new List<Item> { item };
                    gotItem = true;
                }
                else if (inventory[i][0].name == item.name && inventory[i].Count < item.stackSize)
                {
                    inventory[i].Add(item);
                    gotItem = true;
                }
            }

            if (!gotItem) break;

            gotItem = false;
            amount--;
        }

        if (startAmount - amount <= 0)
        {
            print("Couldn't give any more items: Inventory Full");
        }
        else
        {
            print("Got " + (startAmount - amount) + " of " + item.name);
        }

        RaiseInvEvent();
    }
    public void GiveItem(int slot, Item item, int amount = 1)
    {
        if(inventory[slot] == null)
        {
            inventory[slot] = new List<Item> { item };
        }
        else if (inventory[slot] != null && inventory[slot][0].name == item.name && inventory[slot].Count < inventory[slot][0].stackSize)
        {
            while (inventory[slot].Count < inventory[slot][0].stackSize && amount > 0)
            {
                inventory[slot].Add(item);
                amount--;
            }
        }

        RaiseInvEvent();
    }

    public void TakeItem(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null && inventory[i][0].name == item.name)
            {
                inventory[i].RemoveAt(0);
                if (inventory[i].Count <= 0) inventory[i] = null;
                break;
            }
        }

        RaiseInvEvent();
    }
    public void TakeItem(int slot)
    {
        if (inventory[slot] != null)
        {
            inventory[slot].RemoveAt(0);
            if (inventory[slot].Count <= 0) inventory[slot] = null;
        }

        RaiseInvEvent();
    }

    //Player Attacks
    public void Attack(Entity target, int dmg)
    {
        target.Hurt(dmg);
    }
}

