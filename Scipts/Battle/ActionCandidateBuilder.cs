using System.Collections.Generic;
using UnityEngine;

public class ActionCandidateBuilder
{
    public List<BattleAction> BuildForPiece(BattleState state, PieceRuntime piece)
    {
        var result = new List<BattleAction>();
        var reachable = MoveRangeCalculator.GetReachableCells(state, piece);

        foreach (var pos in reachable)
        {
            var cell = state.board.GetCell(pos);

            var action = new MoveAction
            {
                actorRuntimeId = piece.runtimeId,
                fromPos = piece.boardPos,
                toPos = pos,
                targetPos = pos,
                actionType = ActionType.Move,
                priority = 0,
                sourceOrder = 0,
                sourceName = piece.displayConfig != null ? piece.displayConfig.pieceName : "Piece"
            };

            if (cell.pieceRuntimeId >= 0)
            {
                var targetPiece = state.GetPiece(cell.pieceRuntimeId);
                if (targetPiece != null && targetPiece.camp != piece.camp)
                {
                    action.actionType = ActionType.Attack;
                }
                else
                {
                    continue;
                }
            }

            result.Add(action);
        }

        return result;
    }

    public List<BattleAction> BuildForCard(BattleState state, CardRuntime card)
    {
        var result = new List<BattleAction>();
        if (card?.config == null) return result;

        if (!string.IsNullOrEmpty(card.config.pieceId))
        {
            for (int x = 0; x < BoardState.Width; x++)
            {
                for (int y = 0; y < BoardState.Height; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (!IsDeployCellForCamp(pos, Camp.Player))
                        continue;

                    if (!state.board.IsOccupied(pos))
                    {
                        result.Add(new SummonAction
                        {
                            actorRuntimeId = -1,
                            cardRuntimeId = card.runtimeId,
                            targetPos = pos,
                            actionType = ActionType.Summon,
                            priority = 0,
                            sourceOrder = card.acquireOrder,
                            sourceName = card.config.cardName
                        });
                    }
                }
            }
        }

        return result;
    }

    private bool IsDeployCellForCamp(Vector2Int pos, Camp camp)
    {
        if (camp == Camp.Player)
            return pos.y >= 0 && pos.y <= 2;

        if (camp == Camp.Enemy)
            return pos.y >= BoardState.Height - 3 && pos.y < BoardState.Height;

        return false;
    }
}
