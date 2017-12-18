using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    public string name;
    public int stackSize;
    public Texture2D tex;
    public Attack effect;

    public Item(string newName, int newStackSize, Texture2D newTex, Attack newEffect)
    {
        name = newName;
        stackSize = newStackSize;
        tex = newTex;
        effect = newEffect;
    }

    public Item(Item clone)
    {
        name = clone.name;
        stackSize = clone.stackSize;
        tex = clone.tex;
        effect = clone.effect;
    }

}
