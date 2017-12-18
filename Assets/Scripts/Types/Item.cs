using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Ingredient, Potion, Bomb }

public class Item {

    protected GameController gc;

    public string name;
    public ItemType type;
    public Sprite sprite;

    public int stackSize, dmg, range, deadRange;

    protected Item()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public virtual void Effect(Entity target, int dmg)
    {

    }
    
}

public class Test : Item
{
    public Test():
        base()
    {
        name = "Test";
        type = ItemType.Bomb;
        sprite = gc.itemSprites[0];

        stackSize = 15;
        dmg = 5;
        range = 1;
        deadRange = 0;
    }

    public override void Effect(Entity target, int dmg)
    {
        base.Effect(target, dmg);

        target.Hurt(dmg);
    }
}
