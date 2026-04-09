using System.Collections.Generic;
using UnityEngine;

public static class MoveRangeCalculator
{
    public static HashSet<Vector2Int> GetReachableCells(BattleState state, PieceRuntime piece)
    {
        var result = new HashSet<Vector2Int>();

        foreach (var layer in piece.layers)
        {
            if (layer.pieceConfig == null) continue;

            foreach (var offset in layer.pieceConfig.moveOffsets)
            {
                Vector2Int delta = new Vector2Int(offset.x, offset.y);

                if (piece.camp == Camp.Enemy)
                    delta = new Vector2Int(delta.x, -delta.y);

                Vector2Int target = piece.boardPos + delta;
                if (!state.board.IsInside(target))
                    continue;

                var cell = state.board.GetCell(target);
                if (cell.buildingRuntimeId >= 0)
                    continue;

                result.Add(target);
            }
        }

        return result;
    }
}