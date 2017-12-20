using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { start, end, other, none }

public class Map {

    private GameController gc;

    public IntVector2 coords;

    public List<List<Tile>> map = new List<List<Tile>>();

    public int width
    {
        get { return map.Count; }
    }

    public int height
    {
        get { return map[0].Count; }
    }

    public Map(List<List<Tile>> newMap = null)
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        if (newMap == null)
        {
            //generate map
            map = new List<List<Tile>>
            {
                new List<Tile> { new Tile(), new Tile.Path(), new Tile.Path() },

                new List<Tile> { new Tile(), new Tile.Start(), new Tile.Battle() },

                new List<Tile> { new Tile(), new Tile.Battle(), new Tile.Path() }
            };
        }
        else
        {
            map = newMap;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gc.mapHandler.AddTile(map[x][y], new IntVector2(x, y));
            }
        }

        MoveToStart();
    }

    public void MoveToStart()
    {        
        for (int x = 0; x < map.Count; x++)
        {
            for (int y = 0; y < map[x].Count; y++)
            {
                if (map[x][y].type == TileType.start)
                {
                    coords = new IntVector2(x, y);
                    gc.mapHandler.UpdatePlayer(coords);
                    return;
                }
            }
        }
    }

    public void SetCoords(int x, int y, bool runFunc = true)
    {
        SetCoords(new IntVector2(x, y), runFunc);
    }

    public void SetCoords(IntVector2 newCoords, bool runFunc = true)
    {
        if (coords.x < width && coords.x >=0
            && coords.y < height && coords.y >= 0
            && map[coords.x][coords.y].type != TileType.none)
        {
            coords = newCoords;
            gc.mapHandler.UpdatePlayer(coords);
            if (runFunc) map[coords.x][coords.y].Func();
        }
    }

    public Tile GetTile(int x, int y)
    {
        return map[x][y];
    }

    public Tile GetTile(IntVector2 _coords)
    {
        return map[_coords.x][_coords.y];
    }

}

[System.Serializable]
public class IntVector2
{
    public int x, y;

    public IntVector2()
    {
        x = y = 0;
    }

    public IntVector2(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public static explicit operator Vector2(IntVector2 iV2)
    {
        return new Vector2(iV2.x, iV2.y);
    }

    public static explicit operator Vector3(IntVector2 iV2)
    {
        return new Vector3(iV2.x, iV2.y, 0);
    }

    public static IntVector2 operator +(IntVector2 one, IntVector2 two)
    {
        return new IntVector2(one.x + two.x, one.y + two.y);
    }
    public static IntVector2 operator -(IntVector2 one, IntVector2 two)
    {
        return new IntVector2(one.x - two.x, one.y - two.y);
    }
    public static IntVector2 operator *(IntVector2 iV2, int i)
    {
        return new IntVector2(iV2.x * i, iV2.y * i);
    }
    public static IntVector2 operator /(IntVector2 iV2, int i)
    {
        return new IntVector2(iV2.x / i, iV2.y / i);
    }
    public static bool operator ==(IntVector2 one, IntVector2 two)
    {
        return (one.x == two.x) && (one.y == two.y);
    }
    public static bool operator !=(IntVector2 one, IntVector2 two)
    {
        return !(one.x == two.x) && (one.y == two.y);
    }

    public static IntVector2 up = new IntVector2(0, 1);
    public static IntVector2 down = new IntVector2(0, -1);
    public static IntVector2 right = new IntVector2(1, 0);
    public static IntVector2 left = new IntVector2(-1, 0);
}
