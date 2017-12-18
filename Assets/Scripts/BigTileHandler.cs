using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTileHandler : MonoBehaviour {

    GameController gc;

    public bool triggered, moving;
    public float tileWidth, walkSpd;

    public List<GameObject> bigTilePrefs;

    private int dir = 1;
    private bool axis;

    private Vector3 targetPos;

    private List<List<GameObject>> bigMap = new List<List<GameObject>>();

    // Use this for initialization
    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Start ()
    {
        foreach (List<Tile> x in gc.currentMap.map)
        {
            bigMap.Add(new List<GameObject>());
            foreach (Tile y in x)
            {
                if (y.type != TileType.none)
                {
                    bigMap[bigMap.Count - 1].Add(Instantiate(bigTilePrefs[0], transform));
                }
                else
                {
                    bigMap[bigMap.Count - 1].Add(null);
                }
            }
        }

        Invoke("GetBigTiles", 0);
	}

    void GetBigTiles()
    {
        for (int x = 0; x < bigMap.Count; x++)
        {
            for (int y = 0; y < bigMap[0].Count; y++)
            {
                if (bigMap[x][y] != null)
                {
                    bigMap[x][y].GetComponent<BigTile>().coords = new IntVector2(x, y);

                    bigMap[x][y].SetActive(false);
                }
            }
        }

        SwitchAxis(true, false);

    }

    // Update is called once per frame
    void Update() {

        BigTile activeChild = CheckChildren();

        if (!triggered) { 
            if (activeChild != null)
            {
                triggered = true;
                targetPos = transform.position - activeChild.transform.position;
                targetPos.y = transform.position.y;

                Debug.Log("---");
                gc.currentMap.SetCoords(activeChild.coords);

                moving = false;
            }
        }
        else
        {
            if (activeChild == null)
            {
                triggered = false;
            }
        }

        if (moving)
        {
            transform.position += Vector3.left * walkSpd * dir * Time.deltaTime;
        }
        else if (activeChild != null)
        { 
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 10 * Time.deltaTime);
        }

	}

    private BigTile CheckChildren()
    {
        // check coords of all child BigTiles
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                Vector3 worldPos = child.transform.position;

                if (worldPos.x >= -gc.center && worldPos.x <= gc.center)
                {
                    return child.GetComponent<BigTile>();
                }
            }
        }

        // if no matching child is found...
        return null;
    }

    public void SwitchAxis() { SwitchAxis(!axis); }
    public void SwitchAxis(bool newAxis, bool changePos = true)
    {
        axis = newAxis;

        int x = gc.currentMap.coords.x;
        int y = gc.currentMap.coords.y;

        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        if (axis) // switching to the vertical axis
        {
            if (changePos)
            {
                gc.currentMap.SetCoords(gc.currentMap.coords + IntVector2.up * dir);
            }

            for (int y2 = 0; y2 < gc.currentMap.width; y2++)
            {
                if (bigMap[x][y2] != null)
                {
                    bigMap[x][y2].SetActive(true);
                    bigMap[x][y2].transform.position = transform.position + new Vector3(y2, 0) * tileWidth * 2;

                    if (bigMap[x][y2].GetComponent<BigTile>().coords == gc.currentMap.coords)
                    {
                        transform.position -= bigMap[x][y2].transform.position;
                        transform.position = new Vector3(transform.position.x, 1, 0);
                    }
                }
            }
        }
        else // switching to the horizontal axis
        {
            if (changePos)
            {
                gc.currentMap.SetCoords(gc.currentMap.coords + IntVector2.right * dir);
            }

            for (int x2 = 0; x2 < gc.currentMap.width; x2++)
            {
                if (bigMap[x2][y] != null)
                {
                    bigMap[x2][y].SetActive(true);
                    bigMap[x2][y].transform.position = transform.position + new Vector3(x2, 0) * tileWidth * 2;

                    if (bigMap[x2][y].GetComponent<BigTile>().coords == gc.currentMap.coords)
                    {
                        transform.position -= bigMap[x2][y].transform.position;
                        transform.position = new Vector3(transform.position.x, 1, 0);
                    }
                }
            }
        }
        
        moving = false;
    }

    // Move along horizontal axis
    public void MoveRight()
    {
        IntVector2 coords = gc.currentMap.coords;
        if (coords.x < gc.currentMap.width - 1 && gc.currentMap.GetTile(coords + IntVector2.right).type != TileType.none)
        {
            if (axis)
            {
                dir = 1;

                gc.FadeOut(SwitchAxis);
            }
            else
            {
                dir = 1;

                moving = true;
            }
        }
    }
    public void MoveLeft()
    {
        IntVector2 coords = gc.currentMap.coords;
        if (coords.x > 0 && gc.currentMap.GetTile(coords + IntVector2.left).type != TileType.none)
        {
            if (axis)
            {
                dir = -1;

                gc.FadeOut(SwitchAxis);
            }
            else
            {
                dir = -1;

                moving = true;
            }
        }
    }

    // Move along vertical axis
    public void MoveUp()
    {
        IntVector2 coords = gc.currentMap.coords;
        if (coords.y < gc.currentMap.height - 1 && gc.currentMap.GetTile(coords + IntVector2.up).type != TileType.none)
        {
            if (!axis)
            {
                dir = 1;

                gc.FadeOut(SwitchAxis);
            }
            else
            {
                dir = 1;

                moving = true;
            }
        }
    }
    public void MoveDown()
    {
        IntVector2 coords = gc.currentMap.coords;
        if (coords.y > 0 && gc.currentMap.GetTile(coords + IntVector2.down).type != TileType.none)
        {
            if (!axis)
            {
                dir = -1;

                gc.FadeOut(SwitchAxis);
            }
            else
            {
                dir = -1;

                moving = true;
            }
        }
    }
}
