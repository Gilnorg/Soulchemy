using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public string type;
    public Texture2D tex;

    public System.Action func;

    public string dialogue;

    public string mapString;

    public Tile() { func = null; type = "noTile"; }

    public Tile(string newType, Texture2D newTex, System.Action newFunc)
    {
        setVars(newType, newTex, newFunc, null, null);
    }
    public Tile(string newType, Texture2D newTex, System.Action newFunc, string newDialogue)
    {
        setVars(newType, newTex, newFunc, newDialogue, null);
    }

    public Tile(Tile clone)
    {
        setVars(clone.type, clone.tex, clone.func, clone.dialogue, clone.mapString);
    }
    public Tile(string newDialogue, Tile clone)
    {
        setVars(clone.type, clone.tex, clone.func, newDialogue, clone.mapString);
    }
    public Tile(Tile clone, string newGate)
    {
        setVars(clone.type, clone.tex, clone.func, clone.dialogue, newGate);
    }
    public Tile(Tile clone, string newDialogue, string newGate)
    {
        setVars(clone.type, clone.tex, clone.func, newDialogue, newGate);
    }

    private void setVars(string newType, Texture2D newTex, System.Action newFunc, string newDialogue, string newGate)
    {
        type = newType;
        tex = newTex;
        func = newFunc;
        dialogue = newDialogue;
        mapString = newGate;
    }
}
