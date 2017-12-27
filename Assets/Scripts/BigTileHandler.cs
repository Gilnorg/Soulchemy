using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis { x, y }
public class BigTileHandler : MonoBehaviour {

    GameController gc;

    public float tileWidth, walkSpeed;

    public Axis currentAxis;

    public Axis notAxis
    {
        get { return (currentAxis == Axis.x) ? Axis.y : Axis.x; }
    }

    public bool triggered, moving;

    public List<GameObject> bigTilePref = new List<GameObject>();

    List<List<BigTile>> bigMap = new List<List<BigTile>>();

    private class BigTile
    {
        public IntVector2 coords;
        public GameObject gameObject;
        public Transform transform;

        public BigTile(int x, int y, GameObject newGameObject)
        {
            coords = new IntVector2(x, y);
            gameObject = newGameObject;
            transform = gameObject.transform;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public bool activeSelf
        {
            get { return gameObject.activeSelf; }
        }
    }

    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start()
    {
        // add a BigTile GameObject to bigMap for each tile in gc.currentMap
        for (int x = 0; x < gc.currentMap.width; x++)
        {
            bigMap.Add(new List<BigTile>());
            for (int y = 0; y < gc.currentMap.height; y++)
            {
                if (gc.currentMap.GetTile(x, y).type == TileType.none)
                {
                    bigMap[x].Add(null);
                }
                else
                {
                    bigMap[x].Add(new BigTile(x, y, Instantiate(bigTilePref[Random.Range(0, bigTilePref.Count)], transform)));
                    bigMap[x][y].SetActive(false);
                    bigMap[x][y].gameObject.name += "(" + x + ", " + y + ")";

                    Thing[] things = gc.currentMap.GetTile(bigMap[x][y].coords).things;
                    int normal = things.Length / 2;

                    for (int i = 0; i < things.Length; i++)
                    {
                        if (things[i] != null)
                        {
                            Instantiate(
                                things[i],
                                bigMap[x][y].transform.position - Vector3.right * (i - normal) * GameController.unitWidth,
                                bigMap[x][y].transform.rotation, bigMap[x][y].transform
                                );
                        }
                    }
                }
            }
        }

        SetAxis(Axis.x);
    }

    // swap axis between vertical and horizontal
    private void ToggleAxis()
    {
        SetAxis(notAxis);
    }

    private void SetAxis(Axis newAxis)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        triggered = false;

        currentAxis = newAxis;

        var x = gc.currentMap.coords.x;
        var y = gc.currentMap.coords.y;

        if (currentAxis == Axis.x)
        {
            foreach (List<BigTile> column in bigMap)
            {
                BigTile bigTile = column[y];

                if (bigTile != null)
                {
                    ActivateBigTile(bigTile, rot.x > 0 ? bigTile.coords.x : gc.currentMap.width - bigTile.coords.x);
                }
            }
        }
        else
        {
            foreach (BigTile bigTile in bigMap[x])
            {
                if (bigTile != null)
                {
                    ActivateBigTile(bigTile, rot.x > 0 ? bigTile.coords.y : gc.currentMap.height - bigTile.coords.y);
                }
            }
        }

        gc.guiC.uiDisabled = false;
    }

    void ActivateBigTile(BigTile bigTile, int pos)
    {
        bigTile.SetActive(true);
        bigTile.transform.position = new Vector3(transform.position.x + pos * tileWidth, transform.position.y);

        if (bigTile.coords == gc.currentMap.coords)
        {
            transform.position -= bigTile.transform.position;
        }
    }


    private void Update()
    {
        BigTile activeTile = null;

        foreach (List<BigTile> column in bigMap)
        {
            foreach (BigTile row in column)
            {
                if (row != null
                    && row.activeSelf
                    && row.transform.position.x >= -gc.center
                    && row.transform.position.x <= gc.center)
                {
                    activeTile = row;
                }
            }
        }

        if (!triggered)
        {
            if (activeTile != null)
            {
                triggered = true;
                moving = false;

                gc.currentMap.SetCoords(activeTile.coords);
                gc.guiC.ToggleButtons(gc.guiC.exploringArrows, new string[4]
                {
                    (gc.currentMap.GetTile(notAxis, rot.y) != null && gc.currentMap.GetTile(notAxis, rot.y).type != TileType.none) ? "Up" : null,
                    gc.currentMap.GetTile(notAxis, -rot.y) != null && gc.currentMap.GetTile(notAxis, -rot.y).type != TileType.none ? "Down" : null,
                    gc.currentMap.GetTile(currentAxis, rot.x) != null && gc.currentMap.GetTile(currentAxis, rot.x).type != TileType.none ? "Right" : null,
                    gc.currentMap.GetTile(currentAxis, -rot.x) != null && gc.currentMap.GetTile(currentAxis, -rot.x).type != TileType.none ? "Left" : null
                }
                );

                transform.position -= activeTile.transform.position;
                transform.position = new Vector3(transform.position.x, 1);
            }
        }
        else
        {
            if (activeTile == null)
            {
                triggered = false;
            }
        }

        if (moving)
        {
            transform.position += Vector3.left * dir * walkSpeed * Time.deltaTime;
        }
    }
    
    public int dir = 1;
    public IntVector2 rot = new IntVector2(1, 1);

    private void Rotate(int dir)
    {        
        if (dir > 0) //rotate counterclockwise
        {
            for (int i = Mathf.Abs(dir); i > 0; i--)
            {
                if (rot.x == rot.y)
                {
                    rot.y *= -1;
                }
                else
                {
                    rot.x = rot.y;
                }

                gc.mapHandler.RotateMap(-90);
            }
        }

        else if (dir < 0) //rotate clockwise
        {
            for (int i = Mathf.Abs(dir); i > 0; i--)
            {
                if (rot.y == rot.x)
                {
                    rot.x *= -1;
                }
                else
                {
                    rot.y = rot.x;
                }

                gc.mapHandler.RotateMap(90);
            }
        }
                
        else return; //no rotation
    }

    // Move Functions
    public void MoveUp()
    {
        var currentRot = rot.y;
        Rotate(1);
        MoveAlongNotAxis(currentRot);
    }

    public void MoveDown()
    {
        var currentRot = rot.y;
        Rotate(-1);
        MoveAlongNotAxis(-currentRot);
    }

    private void MoveAlongNotAxis(int dir)
    {
        IntVector2 currentCoords = notAxis == Axis.x ? new IntVector2(dir, 0) + gc.currentMap.coords : new IntVector2(0, dir) + gc.currentMap.coords;

        if (gc.currentMap.GetTile(notAxis, dir) != null)
        {
            gc.currentMap.SetCoords(currentCoords, false);
            gc.FadeOut(ToggleAxis);
        }

        gc.guiC.uiDisabled = true;
    }

    public void MoveRight()
    {
        dir = 1;

        if (gc.currentMap.GetTile(currentAxis, rot.x) != null)
        {
            moving = true;
        }
    }

    public void MoveLeft()
    {
        dir = -1;

        if (gc.currentMap.GetTile(currentAxis, -rot.x) != null)
        {
            moving = true;
        }
    }

}
