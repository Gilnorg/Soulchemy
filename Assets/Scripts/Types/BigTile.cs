using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTile : MonoBehaviour {

    public IntVector2 coords;

    private IntVector2 tempCoords;

    private void Awake()
    {
        tempCoords = coords;
    }

    private void Update()
    {
        if (tempCoords != coords)
        {
            tempCoords = coords;
            print("Whut");
        }
    }

}
