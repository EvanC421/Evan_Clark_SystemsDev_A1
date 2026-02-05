using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{

    public TetrisManager tetrisManager;
    public Piece PrefabPiece;
    public TetronimoData[] tetronimos;
    private Piece activePiece;
    public Tilemap tilemap;
    public Vector2Int boardSize;
    public Vector2Int startPosition;


    //This array keeps track of the order in which pieces fall to complete the puzzle
    public Tetronimo[] pieceOrder = { Tetronimo.H, Tetronimo.I, Tetronimo.H };
    public Tetronimo[] newPieceOrder = { Tetronimo.H, Tetronimo.I, Tetronimo.H, Tetronimo.H };

    //This is the REAL array, not like those FAKERS above me
    public Tetronimo[] finalPieceOrder = { Tetronimo.H, Tetronimo.I, Tetronimo.H, Tetronimo.H };
    int t = 0;

    public float dropInterval = 0.5f;
    float dropTime = 0.0f;

    int left
    {
        get { return -boardSize.x / 2; }
    }
    int right
    {
        get { return boardSize.x / 2; }
    }

    int top
    {
        get { return boardSize.y / 2; }
    }
    int bottom
    {
        get { return -boardSize.y / 2; }
    }

    private void Update()
    {
        if (tetrisManager.gameOver) return;

        dropTime += Time.deltaTime;

        if (dropTime >= dropInterval)
        {
            dropTime = 0.0f;

            Clear(activePiece);
            bool moveResult = activePiece.Move(Vector2Int.down);
            Set(activePiece);

            if (!moveResult)
            {
                activePiece.freeze = true;
                CheckBoard();
                SpawnPiece();
            }
        }
    }

    public void SpawnPiece()
    {
        if (t >= 0 && t < finalPieceOrder.Length)
        {

            activePiece = Instantiate(PrefabPiece);

            //Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);

            activePiece.Initialize(this, finalPieceOrder[t]);
            CheckEndGame();
            Set(activePiece);
            t += 1;
        }

        else tetrisManager.SetGameOver(true);
    }

    void CheckEndGame()
    {
        if (!IsPositionValid(activePiece, activePiece.position))
        {
            //if there is not a valid position for the newly placed piece, game over
            tetrisManager.SetGameOver(true);
        }
    }

    public void UpdateGameOver()
    {
        if (!tetrisManager.gameOver)
        {
            ResetBoard();
        }
    }

    void ResetBoard()
    {
        Piece[] foundPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);

        foreach (Piece piece in foundPieces) Destroy(piece.gameObject);
        activePiece = null;
        tilemap.ClearAllTiles();

        t = 0;
        SpawnPiece();
        SpawnGreyCells();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            tilemap.SetTile(cellPosition, null);
        }
    }

    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        int left = -boardSize.x / 2;
        int right = boardSize.x / 2;
        int bottom = -boardSize.y / 2;
        int top = boardSize.y / 2;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);

            if (cellPosition.x < left || cellPosition.x >= right ||
                cellPosition.y < bottom || cellPosition.y >= top)
            {
                return false;
            }

            if (tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    bool IsLineFull(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);
            if (!tilemap.HasTile(cellPosition)) return false;
        }

        return true;
    }

    void DestroyLine(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);
            tilemap.SetTile(cellPosition, null);
        }
    }

    void ShiftRowsDown(int clearedLine)
    {
        for (int y = clearedLine + 1; y < top; y++)
        {
            for (int x = left; x < right; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);

                //temp save tile
                TileBase currentTile = tilemap.GetTile(cellPosition);

                tilemap.SetTile(cellPosition, null);

                cellPosition.y -= 1;
                tilemap.SetTile(cellPosition, currentTile);
            }
        }
    }

    public void CheckBoard()
    {
        List<int> destroyedLines = new List<int>();
        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {

                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }

        int rowsShiftedDown = 0;
        foreach (int y in destroyedLines)
        {
            ShiftRowsDown(y - rowsShiftedDown);
            rowsShiftedDown++;
        }

        int score = tetrisManager.CalculateScore(destroyedLines.Count);
        tetrisManager.changeScore(score);

    }
    //Right,so you call them grey cells, despite the fact that they are obviously green.
    //(They're actually whatever colour the first piece in pieceOrder is)
    public void SpawnGreyCells()
    {

        List<Vector3Int> GreyCells = new List<Vector3Int>()
        {
            //These are the locations of every cell that needs to be spawned for the puzzle.
            //Row one (bottom row)
            new Vector3Int(left +0, bottom+ 0,0),
            new Vector3Int(left +1, bottom+ 0,0),
            new Vector3Int(left +2, bottom+ 0,0),
            new Vector3Int(left +3, bottom+ 0,0),
            new Vector3Int(left +4, bottom+ 0,0),
            new Vector3Int(left +5, bottom+ 0,0),
            new Vector3Int(left +9, bottom+ 0,0),

            //Row two
            new Vector3Int(left +0, bottom+ 1,0),
            new Vector3Int(left +1, bottom+ 1,0),
            new Vector3Int(left +2, bottom+ 1,0),
            new Vector3Int(left +3, bottom+ 1,0),
            new Vector3Int(left +4, bottom+ 1,0),
            new Vector3Int(left +5, bottom+ 1,0),
            new Vector3Int(left +9, bottom+ 1,0),

            //Row three
            new Vector3Int(left +0, bottom+ 2,0),
            new Vector3Int(left +1, bottom+ 2,0),
            new Vector3Int(left +2, bottom+ 2,0),
            new Vector3Int(left +3, bottom+ 2,0),
            new Vector3Int(left +4, bottom+ 2,0),
            new Vector3Int(left +5, bottom+ 2,0),
            new Vector3Int(left +9, bottom+ 2,0),

            //Row four
            new Vector3Int(left +0, bottom+ 3,0),
            new Vector3Int(left +1, bottom+ 3,0),
            new Vector3Int(left +3, bottom+ 3,0),
            new Vector3Int(left +4, bottom+ 3,0),
            new Vector3Int(left +5, bottom+ 3,0),
            new Vector3Int(left +6, bottom+ 3,0),
            new Vector3Int(left +7, bottom+ 3,0),
            new Vector3Int(left +8, bottom+ 3,0),
            new Vector3Int(left +9, bottom+ 3,0),

            //Row five
            new Vector3Int(left +0, bottom+ 4,0),
            new Vector3Int(left +2, bottom+ 4,0),
            new Vector3Int(left +4, bottom+ 4,0),
            new Vector3Int(left +5, bottom+ 4,0),
            new Vector3Int(left +6, bottom+ 4,0),
            new Vector3Int(left +7, bottom+ 4,0),
            new Vector3Int(left +8, bottom+ 4,0),
            new Vector3Int(left +9, bottom+ 4,0),

            //Row six
            new Vector3Int(left +0, bottom+ 5,0),
            new Vector3Int(left +4, bottom+ 5,0),
            new Vector3Int(left +5, bottom+ 5,0),
            new Vector3Int(left +6, bottom+ 5,0),
            new Vector3Int(left +7, bottom+ 5,0),
            new Vector3Int(left +8, bottom+ 5,0),
            new Vector3Int(left +9, bottom+ 5,0),

            //Row seven (top row)
            new Vector3Int(left +0, bottom+ 6,0),
            new Vector3Int(left +4, bottom+ 6,0),
            new Vector3Int(left +5, bottom+ 6,0),
            new Vector3Int(left +6, bottom+ 6,0),
            new Vector3Int(left +7, bottom+ 6,0),
            new Vector3Int(left +8, bottom+ 6,0),
            new Vector3Int(left +9, bottom+ 6,0),
        };



        foreach (var cell in GreyCells)
        {
            activePiece.Initialize(this, Tetronimo.G);
            tilemap.SetTile(cell, activePiece.data.tile);

        }
    }


}
