using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapHandler : MonoBehaviour {

    private GameController gc;
    private GameObject playerMark;

    public GameObject container, display;
    public GameObject tilePref, playerMarkPref;

    public float tileWidth;

	// Use this for initialization
	void Awake () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        playerMark = Instantiate(playerMarkPref, container.transform);
	}
	
	public void AddTile(Tile tile, IntVector2 pos)
    {
        if (tile.type != TileType.none)
        {
            var newTile = Instantiate(tilePref, container.transform);

            newTile.transform.position += (Vector3)pos * tileWidth;
            newTile.GetComponent<SpriteRenderer>().sprite = tile.sprite;
        }
    }

    public void ToggleMap()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void RotateMap(Vector3 newRotation)
    {
        display.transform.Rotate(newRotation);
    }

    public void UpdatePlayer(IntVector2 pos)
    {
        playerMark.transform.position = container.transform.position + (Vector3)pos * tileWidth;
    }
}
