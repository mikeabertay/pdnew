using UnityEngine;

public abstract class BattleAction
{
    public int actorRuntimeId;
    public Vector2Int targetPos;
    public ActionType actionType;
    public int priority;
    public int sourceOrder;
    public string sourceName;

    public abstract bool CanExecute(BattleState state);
}


public class MoveAction : BattleAction
{
    public Vector2Int fromPos;
    public Vector2Int toPos;

    public override bool CanExecute(BattleState state)
    {
        return state.board.IsInside(toPos);
    }
}

public class SummonAction : BattleAction
{
    public int cardRuntimeId;

    public override bool CanExecute(BattleState state)
    {
        return state.board.IsInside(targetPos) && !state.board.IsOccupied(targetPos);
    }
}

public class UpgradeAction : BattleAction
{
    public int cardRuntimeId;
    public int targetPieceRuntimeId;

    public override bool CanExecute(BattleState state)
    {
        var piece = state.GetPiece(targetPieceRuntimeId);
        return piece != null && !piece.isDead;
    }
}