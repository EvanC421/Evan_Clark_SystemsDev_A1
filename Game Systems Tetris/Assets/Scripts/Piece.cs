using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    public TetrisManager tetrisManager;
    public TetronimoData data;
    public Board board;
    public Vector2Int[] cells;

    public Vector2Int position;

    public bool freeze = false;

    public void Initialize(Board board, Tetronimo tetronimo)
    {
        this.board = board;

        for (int i = 0; i < board.tetronimos.Length; i++)
        {
            if (board.tetronimos[i].tetronimo == tetronimo)
            {
                this.data = board.tetronimos[i];
                break;
            }

        }
        cells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < data.cells.Length; i++) cells[i] = data.cells[i];

        position = board.startPosition;
    }

    public void InitializeGrey(Board board, Tetronimo tetronimo)
    {
        this.board = board;

        for (int i = 0; i < board.tetronimos.Length; i++)
        {
            if (board.tetronimos[i].tetronimo == tetronimo)
            {
                this.data = board.tetronimos[i];
                break;
            }

        }
        cells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < data.cells.Length; i++) cells[i] = data.cells[i];
        position = board.startPosition;
    }

    private void Update()
    {
        if (board.tetrisManager.gameOver) return;
        if (freeze) return;
        board.Clear(this);

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        //else if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Move(Vector2Int.up);
        //}
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rotate(1);
        }
        else if ( Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rotate(-1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

            board.Set(this);


        if (freeze)
        {
            board.CheckBoard();
            board.SpawnPiece();
        }

        //DEBUG ONLY, DELETE BEFORE RELEASE 
        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CheckBoard();
        }
    }

    public bool Move(Vector2Int translation)
    {
        Vector2Int newPosition = position;
        newPosition += translation;

        bool PositionValid = board.IsPositionValid(this, newPosition);
        if (board.IsPositionValid(this, newPosition)) {
            position = newPosition;
        }

        return PositionValid;
    }

    void HardDrop()
    {
        while (Move(Vector2Int.down))
        {

        }
        freeze = true;
    }

    void Rotate(int direction)
    {
        Vector2Int[] OGCells = new Vector2Int[cells.Length];

        for (int i = 0; i <cells.Length; i++)
        {
            OGCells[i] = cells[i];
        }

        ApplyRotation(direction);

        if (!board.IsPositionValid(this, position))
        {
            if (!TryWallKicks())
            {
                RevertRotation(OGCells);
            }
            else
            {
                Debug.Log("WallKicks succeeded");
            }
        }
        
    }

    void RevertRotation(Vector2Int[] OGCells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = OGCells[i];
        }
    }

    bool TryWallKicks()
    {
        List<Vector2Int> wallKickOffsets = new List<Vector2Int>
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down,
            new Vector2Int(-1,-1),
            new Vector2Int(1,-1),
        };

        if (data.tetronimo == Tetronimo.I)
        {
            wallKickOffsets.Add(2*Vector2Int.left);
            wallKickOffsets.Add(2 * Vector2Int.right);
        }

        foreach (Vector2Int offset in wallKickOffsets)
        {
            if (Move(offset)) return true;
        }

        return false;
    }

    void ApplyRotation(int direction)
    {

        bool isSpecial = data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.O;
        Quaternion rotation = Quaternion.Euler(0, 0, 90 * direction);
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cellPosition = new Vector3(cells[i].x, cells[i].y);

            if (isSpecial)
            {
                cellPosition.x -= 0.5f;
                cellPosition.y -= 0.5f;
            }

            Vector3 result = rotation * cellPosition;

            if (isSpecial)
            {
                cells[i].x = Mathf.CeilToInt(result.x);
                cells[i].y = Mathf.CeilToInt(result.y);
            }
            else
            {
                cells[i].x = Mathf.RoundToInt(result.x);
                cells[i].y = Mathf.RoundToInt(result.y);
            }
        }
    }
}
