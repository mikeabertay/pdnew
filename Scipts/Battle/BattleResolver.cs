using System.Collections.Generic;
using UnityEngine;

public class BattleResolver
{
    public List<ResolutionStep> Resolve(BattleState state, BattleAction action)
    {
        var steps = new List<ResolutionStep>();

        if (!action.CanExecute(state))
            return steps;

        switch (action)
        {
            case MoveAction moveAction:
                ResolveMove(state, moveAction, steps);
                break;
            case SummonAction summonAction:
                ResolveSummon(state, summonAction, steps);
                break;
            case UpgradeAction upgradeAction:
                ResolveUpgrade(state, upgradeAction, steps);
                break;
        }

        return steps;
    }

    private void ResolveMove(BattleState state, MoveAction action, List<ResolutionStep> steps)
    {
        var piece = state.GetPiece(action.actorRuntimeId);
        if (piece == null) return;

        var targetCell = state.board.GetCell(action.toPos);

        if (targetCell.pieceRuntimeId >= 0)
        {
            var targetPiece = state.GetPiece(targetCell.pieceRuntimeId);
            if (targetPiece != null && targetPiece.camp != piece.camp)
            {
                state.triggerQueue.Enqueue(new TriggerEvent
                {
                    triggerType = TriggerType.OnAttack,
                    sourceRuntimeId = piece.runtimeId,
                    targetRuntimeId = targetPiece.runtimeId
                });

                targetPiece.isDead = true;
                state.board.ClearPiece(targetPiece.boardPos);

                steps.Add(new KillStep
                {
                    targetRuntimeId = targetPiece.runtimeId
                });

                state.triggerQueue.Enqueue(new TriggerEvent
                {
                    triggerType = TriggerType.OnKill,
                    sourceRuntimeId = piece.runtimeId,
                    targetRuntimeId = targetPiece.runtimeId
                });

                state.triggerQueue.Enqueue(new TriggerEvent
                {
                    triggerType = TriggerType.OnDeath,
                    sourceRuntimeId = targetPiece.runtimeId,
                    targetRuntimeId = piece.runtimeId
                });
            }
        }

        state.board.ClearPiece(piece.boardPos);

        Vector2Int from = piece.boardPos;
        piece.boardPos = action.toPos;
        state.board.SetPiece(piece.boardPos, piece.runtimeId);

        steps.Add(new MoveStep
        {
            pieceRuntimeId = piece.runtimeId,
            from = from,
            to = piece.boardPos
        });

        state.triggerQueue.Enqueue(new TriggerEvent
        {
            triggerType = TriggerType.OnMove,
            sourceRuntimeId = piece.runtimeId
        });
    }

    private void ResolveSummon(BattleState state, SummonAction action, List<ResolutionStep> steps)
    {
        // ×îÐ¡°æÏÈÁô¿Õ£¬ºóÃæ½ÓÈë PieceRuntime ÊµÀý»¯Âß¼­
    }

    private void ResolveUpgrade(BattleState state, UpgradeAction action, List<ResolutionStep> steps)
    {
        // ×îÐ¡°æÏÈÎ¯ÍÐ¸ø UpgradeResolver
    }
}