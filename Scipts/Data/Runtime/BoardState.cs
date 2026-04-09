using UnityEngine;

public class CellState
{
    public Vector2Int pos;
    public TerrainType terrainType;
    public bool isPlayerBase;
    public bool isEnemyBase;
    public int pieceRuntimeId = -1;
    public int buildingRuntimeId = -1;
}

public class BoardState
{
    public const int Width = 9;
    public const int Height = 9;

    private readonly CellState[,] _cells = new CellState[Width, Height];

    public BoardState()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _cells[x, y] = new CellState
                {
                    pos = new Vector2Int(x, y),
                    terrainType = TerrainType.Normal
                };
            }
        }
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    }

    public CellState GetCell(Vector2Int pos)
    {
        if (!IsInside(pos)) return null;
        return _cells[pos.x, pos.y];
    }

    public bool IsOccupied(Vector2Int pos)
    {
        var cell = GetCell(pos);
        return cell != null && cell.pieceRuntimeId >= 0;
    }

    public void SetPiece(Vector2Int pos, int runtimeId)
    {
        var cell = GetCell(pos);
        if (cell != null) cell.pieceRuntimeId = runtimeId;
    }

    public void ClearPiece(Vector2Int pos)
    {
        var cell = GetCell(pos);
        if (cell != null) cell.pieceRuntimeId = -1;
    }
}