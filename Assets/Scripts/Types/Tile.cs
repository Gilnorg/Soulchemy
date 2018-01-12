using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    protected GameController gc;

    public GameObject gameObject;
    public Transform transform;

    public TileType type = TileType.none;

    public Sprite sprite;

    public bool triggered;

    public SetPiece[] setPieces = new SetPiece[6];

    public Tile(GameObject newGameObject = null)
    {
        gc = GameController.main;

        gameObject = Object.Instantiate(newGameObject, gc.bigTileHandler.transform);
        if (gameObject != null)
        {
            transform = gameObject.transform;
        }

        gameObject.SetActive(false);
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
