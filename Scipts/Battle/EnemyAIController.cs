using System.Collections.Generic;

public class EnemyAIController
{
    private readonly ActionCandidateBuilder _builder = new();
    private readonly ActionAvailabilityService _availabilityService = new();

    public BattleAction Decide(BattleState state)
    {
        BattleAction bestAction = null;
        int bestScore = int.MinValue;

        foreach (var kv in state.pieces)
        {
            var piece = kv.Value;
            if (piece == null || piece.isDead || piece.camp != Camp.Enemy)
                continue;

            if (!_availabilityService.CanPieceAct(state, piece))
                continue;

            List<BattleAction> actions = _builder.BuildForPiece(state, piece);
            foreach (var action in actions)
            {
                if (action == null || !action.CanExecute(state))
                    continue;

                int score = EvaluateAction(state, action);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestAction = action;
                }
            }
        }

        return bestAction;
    }

    private int EvaluateAction(BattleState state, BattleAction action)
    {
        int score = 0;

        if (action.actionType == ActionType.Attack)
            score += 1000;

        if (WouldReachPlayerBase(state, action))
            score += 100000;

        return score;
    }

    private bool WouldReachPlayerBase(BattleState state, BattleAction action)
    {
        var cell = state.board.GetCell(action.targetPos);
        return cell != null && cell.isPlayerBase;
    }
}
