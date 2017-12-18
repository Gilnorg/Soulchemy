using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public List<List<Tile>> map = new List<List<Tile>>();
    public bool isCheckpoint;
    public string name;

    public Map()
    {
        setVars("", new List<List<Tile>>(), false);
    }

    public Map(bool newIsCheckpoint)
    {
        //TO BE IMPLEMENTED: Generate random map
    }

    public Map(string newName, List<List<Tile>> newMap, bool newIsCheckpoint)
    {
        setVars(newName, newMap, newIsCheckpoint);
    }

    public Map(Map clone)
    {
        List<List<Tile>> newMap = new List<List<Tile>>();
        foreach (List<Tile> list in clone.map)
        {
            List<Tile> temp = new List<Tile>();
            foreach (Tile tile in list)
            {
                temp.Add(new Tile(tile));
            }
            newMap.Add(temp);
        }

        setVars(clone.name, newMap, clone.isCheckpoint);
    }

    private void setVars(string newName, List<List<Tile>> newMap, bool newIsCheckpoint)
    {
        name = newName;
        map = newMap;
        isCheckpoint = newIsCheckpoint;
        
        Exploring.allMaps.Add(this);
    }
}
