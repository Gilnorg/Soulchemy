﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    protected GameController gc;

    public TileType type = TileType.none;

    public Sprite sprite;

    public bool triggered;

    public Thing[] things = new Thing[6];

    public Tile()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public virtual void Func()
    {
        triggered = true;
    }

    public class Path : Tile
    {
        public Path() : base()
        {
            type = TileType.other;

            sprite = gc.itemSprites[0];
        }
    }

    public class Start : Tile
    {
        public Start() : base()
        {
            type = TileType.start;

            sprite = gc.itemSprites[0];
        }
    }

    public class End : Tile
    {
        public End() : base()
        {
            type = TileType.end;

            sprite = gc.itemSprites[0];
        }
    }

    public class Battle : Tile
    {
        private int battle;

        public Battle(int newBattle = 0) : base()
        {
            type = TileType.other;

            battle = newBattle;

            sprite = gc.itemSprites[0];
        }

        public override void Func()
        {
            if (!triggered)
            {
                gc.StartCoroutine(gc.BattleTrigger(battle));
            }
            base.Func();
        }
    }
}
