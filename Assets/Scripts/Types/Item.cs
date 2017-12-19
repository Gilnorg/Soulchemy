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

    protected Entity target
    {
        get { return gc.player.currentTarget; }
    }

    protected Item()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public override string ToString()
    {
        return name;
    }
    
    public virtual void Func()
    {

    }

    public class Test : Item
    {
        public Test() :
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

        public override void Func()
        {
            base.Func();

            gc.currentBattle.SplashAttack(target.loc, dmg, gc.Attack, range, deadRange);
        }
    }

}
