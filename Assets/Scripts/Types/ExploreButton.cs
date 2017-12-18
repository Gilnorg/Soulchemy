using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreButton : MonoBehaviour {

    GameController gc;

    public int dir;

	// Use this for initialization
	void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}
	
	public void Move()
    {
        switch (dir)
        {
            default:
                gc.bigTileHandler.MoveUp();
                break;
            case 1:
                gc.bigTileHandler.MoveRight();
                break;
            case 2:
                gc.bigTileHandler.MoveDown();
                break;
            case 3:
                gc.bigTileHandler.MoveLeft();
                break;
        }
    }
}
