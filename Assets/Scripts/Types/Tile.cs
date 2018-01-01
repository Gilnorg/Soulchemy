using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    protected GameController gc;

    public TileType type = TileType.none;

    public Sprite sprite;

    public bool triggered;

    public SetPiece[] things = new SetPiece[6];

    public Tile()
    {
        gc = GameController.main;

        //things[0] = new SetPiece(gc.setPiecePrefs[0]);
        things[1] = new SetPiece(gc.setPiecePrefs[0]);
        //things[2] = new SetPiece(gc.setPiecePrefs[0]);
        things[3] = new SetPiece(gc.setPiecePrefs[0]);
        //things[4] = new SetPiece(gc.setPiecePrefs[0]);
        things[5] = new SetPiece(gc.setPiecePrefs[0]);
    }

    public virtual void Func()
    {
        triggered = true;
    }

    public class SetPiece
    {
        public GameObject gameObject;

        public SetPiece(GameObject newGM)
        {
            gameObject = newGM;
        }

        public virtual void Func(Entity target)
        {
            Debug.Log("???");
        }
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
