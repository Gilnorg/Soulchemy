using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploring : GameHandler {

    public static Vector2 coords = new Vector2(0, 0);

    private static Tile n = new Tile("normal", gameHandler.tileTexList[0], tileNormal);
    private static Tile s = new Tile("start", gameHandler.tileTexList[0], tileNormal);
    private static Tile e = new Tile("end", gameHandler.tileTexList[1], tileEnd);
    private static Tile t = new Tile("treasure", gameHandler.tileTexList[0], tileTreasure);
    private static Tile b = new Tile("battle", gameHandler.tileTexList[2], tileBattle);

    public static List<Map> allMaps = new List<Map>();

    public static Map defaultMap = new Map(
            "defaultMap", new List<List<Tile>> {
            new List<Tile> { new Tile(), new Tile(e, "testMap"), new Tile() },
            new List<Tile> { new Tile(n), new Tile(s), new Tile(n) },
            new List<Tile> { new Tile(), new Tile(n), new Tile() }
            },
            true
        );

    public static Map testMap = new Map("testMap", new List<List<Tile>>
        {
            new List<Tile> { new Tile(),new Tile(e, "defaultMap"),new Tile(),new Tile(),new Tile(n) },

            new List<Tile> { new Tile(),new Tile(n),new Tile(),new Tile(),new Tile() },

            new List<Tile> { new Tile(),new Tile("test", n),new Tile(),new Tile(),new Tile() },

            new List<Tile> { new Tile(),new Tile(n),new Tile(n),new Tile(b),new Tile(s) },

            new List<Tile> { new Tile(n),new Tile(n),new Tile(),new Tile(),new Tile() }
        },
        false
        );

    public static Map currentMap = defaultMap;

    public static Map getMap(string mapString)
    {
        foreach (Map map in allMaps)
        {
            if (map.name == mapString)
            {
                
                return map;
            }
        }
        return null;
    }

    public static void tileNormal()
    {

    }
    public static void tileStart()
    {

    }
    public static void tileEnd()
    {
        
    }
    public static void tileTreasure()
    {

    }
    public static void tileBattle()
    {
        Combat.battleTrigger(Combat.listOfBattles[Mathf.FloorToInt(Random.Range(0, Combat.listOfBattles.Count - 0.1f))]);
    }
    public static void tileLoop()
    {

    }

    //Move Functions
    public static void embark(Map newMap)
    {
        currentMap = new Map(newMap);
        moveToStart();
    }

    public static void doneExploring()
    {
        embark(defaultMap);
    }

    public static void activateTile() { currentMap.map[(int)coords.y][(int)coords.x].func(); }

    public static Tile getCurrentTile() { return currentMap.map[(int)coords.y][(int)coords.x]; }

    public static int getCurrentArea() { return currentMap.map.Count * currentMap.map[0].Count; }

    public static void moveToStart()
    {
        for (int y = 0; y < currentMap.map.Count; y++)
        {
            for (int x = 0; x < currentMap.map[y].Count; x++)
            {
                if (currentMap.map[y][x].type == "start") coords = new Vector2(x, y);
            }
        }

    }

    public static bool adjecentToTile(string loc)
    {
        if (loc == "up")
        {
            if (coords.y - 1 >= 0 && currentMap.map[(int)coords.y - 1][(int)coords.x].type != "noTile")
            {
                return true;
            }
            else return false;
        }
        else if (loc == "down")
        {
            if (coords.y + 1 < currentMap.map.Count && currentMap.map[(int)coords.y + 1][(int)coords.x].type != "noTile")
            {
                return true;
            }
            else return false;
        }
        else if (loc == "left")
        {
            if (coords.x - 1 >= 0 && currentMap.map[(int)coords.y][(int)coords.x - 1].type != "noTile")
            {
                return true;
            }
            else return false;
        }
        else if (loc == "right")
        {
            if (coords.x + 1 < currentMap.map[0].Count && currentMap.map[(int)coords.y][(int)coords.x + 1].type != "noTile")
            {
                return true;
            }
            else return false;
        }
        Debug.LogError("Direction " + loc + " doesn't exist. Must be up, down, left, or right.");
        return false;
    }
}
