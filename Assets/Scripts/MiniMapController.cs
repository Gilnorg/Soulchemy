using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour {

    public GameObject container, display, cam;
    public Toggle toggle;
    public GameObject tilePref, playerMarkPref;

    private GameController gc;
    private GameObject playerMark;

    public float tileWidth;
    
    public float moveSpd, rotSpd;

    private float rotationZ = 0;

    private Vector3 rotation
    {
        get { return new Vector3(0, 0, rotationZ); }
    }

    private Vector3 startPos = new Vector3();

    private float moveDist
    {
        get { return Mathf.Clamp(Vector3.Distance(cam.transform.position, playerMark.transform.position) / Vector3.Distance(startPos, playerMark.transform.position), 0.01f, 1); }
    }

	// Use this for initialization
	void Awake () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        playerMark = Instantiate(playerMarkPref, container.transform);

        startPos = cam.transform.position;

        ToggleMap();
	}

    void Update()
    {
        float angleDist = Mathf.Clamp(Quaternion.Angle(display.transform.rotation, Quaternion.Euler(rotation)) / 45, 0.01f, 1);

        display.transform.rotation = Quaternion.RotateTowards(display.transform.rotation, Quaternion.Euler(rotation), rotSpd * angleDist * Time.deltaTime);
        playerMark.transform.rotation = Quaternion.RotateTowards(playerMark.transform.rotation, Quaternion.Euler(-rotation), rotSpd * angleDist * Time.deltaTime);

        cam.transform.position = Vector3.MoveTowards(cam.transform.position, playerMark.transform.position + new Vector3(0, 0, -10), moveSpd * moveDist * Time.deltaTime);
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
        display.SetActive(toggle.isOn);
    }

    public void RotateMap(int newRot)
    {
        display.transform.eulerAngles = rotation;
        playerMark.transform.eulerAngles = -rotation;

        rotationZ += newRot;
    }

    public void UpdatePlayer(IntVector2 pos)
    {
        startPos = cam.transform.position;
        playerMark.transform.position = container.transform.position + (Vector3)pos * tileWidth;
    }
}
