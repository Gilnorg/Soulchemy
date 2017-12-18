using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOutHandler : MonoBehaviour {

    GameController gc;

	// Use this for initialization
	void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}

    public void RunFunc()
    {
        gc.fadeOutFunc();
    }
	
	
}
