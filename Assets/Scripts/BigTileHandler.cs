using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTileHandler : MonoBehaviour {

    GameController gc;

    public float tileWidth, walkSpeed;

    public enum Axis { x, y }
    public Axis currentAxis;

    public int dir = 1;

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
                }
            }
        }

        SetAxis(Axis.x);
    }

    // swap axis between vertical and horizontal
    private void ToggleAxis()
    {
        if (currentAxis == Axis.x) SetAxis(Axis.y);
        else SetAxis(Axis.x);
    }

    private void SetAxis(Axis newAxis)
    {
        triggered = false;

        currentAxis = newAxis;

        var x = gc.currentMap.coords.x;
        var y = gc.currentMap.coords.y;

        if (currentAxis == Axis.x)
        {
            foreach (List<BigTile> column in bigMap)
            {
                if (column[y] != null)
                {
                    ActivateBigTile(column[y], column[y].coords.y);
                }
            }
        }
        else
        {
            foreach (BigTile row in bigMap[gc.currentMap.coords.x])
            {
                if (row != null)
                {
                    ActivateBigTile(row, row.coords.x);
                }
            }
        }

    }

    void ActivateBigTile(BigTile bigTile, int pos)
    {
        bigTile.SetActive(true);
        bigTile.gameObject.transform.position = new Vector3(transform.position.x - pos * tileWidth, 0);

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

                gc.currentMap.SetCoords(activeTile.coords);

                transform.position = activeTile.transform.position;
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


    // Move Functions
    public void MoveUp()
    {
        dir = 1;

        if (currentAxis == Axis.x)
        {
            gc.currentMap.SetCoords(gc.currentMap.coords + IntVector2.up, false);
            gc.FadeOut(ToggleAxis);
        }
    }

    public void MoveDown()
    {
        dir = -1;

        if (currentAxis == Axis.x)
        {
            gc.currentMap.SetCoords(gc.currentMap.coords + IntVector2.down, false);
            gc.FadeOut(ToggleAxis);
        }
    }

    public void MoveRight()
    {
        dir = 1;

        if (currentAxis == Axis.y)
        {
            gc.currentMap.SetCoords(gc.currentMap.coords + IntVector2.right, false);
            gc.FadeOut(ToggleAxis);
        }
    }

    public void MoveLeft()
    {
        dir = -1;

        if (currentAxis == Axis.y)
        {
            gc.currentMap.SetCoords(gc.currentMap.coords + IntVector2.left, false);
            gc.FadeOut(ToggleAxis);
        }
    }

}
