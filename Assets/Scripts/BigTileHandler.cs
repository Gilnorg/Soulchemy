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

    private void Start()
    {
        gc = GameController.main;

        SetAxis(Axis.x);
    }

    public IEnumerator SpawnSetPiece(GameObject setPiece, int x, int y, int loc)
    {
        GameObject instantiatedSetPiece = Instantiate(setPiece, gc.currentMap.GetTile(x, y).transform);

        yield return 0;

        SetPiece instantiatedSetPieceComponent = instantiatedSetPiece.GetComponent<SetPiece>();

        gc.currentMap.GetTile(x, y).setPieces[loc] = instantiatedSetPieceComponent;
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
            foreach (List<Tile> column in bigMap)
            {
                Tile bigTile = column[y];

                if (bigTile != null)
                {
                    ActivateBigTile(bigTile, rot.x > 0 ? bigTile.coords.x : gc.currentMap.width - bigTile.coords.x);
                }
            }
        }
        else
        {
            foreach (Tile bigTile in bigMap[x])
            {
                if (bigTile != null)
                {
                    ActivateBigTile(bigTile, rot.x > 0 ? bigTile.coords.y : gc.currentMap.height - bigTile.coords.y);
                }
            }
        }

        gc.guiC.uiDisabled = false;
    }

    void ActivateBigTile(Tile bigTile, int pos)
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
        Tile activeTile = null;

        foreach (List<Tile> column in bigMap)
        {
            foreach (Tile row in column)
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
