using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleState State { get; private set; }

    private readonly ActionCandidateBuilder _candidateBuilder = new();
    private readonly BattleResolver _resolver = new();
    private readonly TriggerDispatcher _triggerDispatcher = new();
    private readonly ActionAvailabilityService _availabilityService = new();

    private EnemyAIController _enemyAI;
    private BattlePresentationPlayer _presentationPlayer;
    private BattleHighlightController _highlightController;

    private void Awake()
    {
        _enemyAI = new EnemyAIController();
        _presentationPlayer = GetComponent<BattlePresentationPlayer>();
        _highlightController = GetComponent<BattleHighlightController>();
    }

    public void StartBattle(BattleState state)
    {
        State = state;
        State.phase = BattlePhase.TurnStart;
        State.currentCamp = Camp.Player;
        State.actionsRemaining = 1;

        RebuildActionRestrictions();
        EnterPlayerInputPhase();
    }

    public List<BattleAction> GetActionsForPiece(int pieceRuntimeId)
    {
        var piece = State.GetPiece(pieceRuntimeId);
        if (piece == null) return new List<BattleAction>();
        return _candidateBuilder.BuildForPiece(State, piece);
    }

    public IEnumerator ExecuteAction(BattleAction action)
    {
        if (State.phase == BattlePhase.Resolving || State.phase == BattlePhase.Animating)
            yield break;

        bool consumedExistingExtraAction = ShouldConsumeExistingExtraAction(action);

        EnterResolvingPhase();

        List<ResolutionStep> steps = _resolver.Resolve(State, action);
        List<ResolutionStep> triggerSteps = _triggerDispatcher.ResolveAll(State);
        steps.AddRange(triggerSteps);

        ConsumeAction(consumedExistingExtraAction);

        State.phase = BattlePhase.Animating;
        yield return _presentationPlayer.Play(steps);

        if (CheckBattleEnd())
        {
            State.phase = BattlePhase.BattleEnd;
            ClearAvailableHighlights();
            yield break;
        }

        AdvanceTurnOrInput();
    }

    public void EndTurn()
    {
        ClearAvailableHighlights();

        State.currentCamp = State.currentCamp == Camp.Player ? Camp.Enemy : Camp.Player;
        State.actionsRemaining = 1;
        State.phase = State.currentCamp == Camp.Player ? BattlePhase.WaitingInput : BattlePhase.TurnStart;

        ResetActedFlags();
        RebuildActionRestrictions();

        if (State.currentCamp == Camp.Enemy)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
        else
        {
            RefreshAvailableHighlights();
        }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        var action = _enemyAI.Decide(State);
        if (action != null)
        {
            yield return ExecuteAction(action);
        }
        else
        {
            EndTurn();
        }
    }

    private void ResetActedFlags()
    {
        foreach (var kv in State.pieces)
        {
            kv.Value.hasActedThisTurn = false;
        }
    }

    private bool CheckBattleEnd()
    {
        bool playerHasPiece = false;
        bool enemyHasPiece = false;
        bool playerReachedEnemyBase = false;
        bool enemyReachedPlayerBase = false;

        foreach (var kv in State.pieces)
        {
            var piece = kv.Value;
            if (piece == null || piece.isDead)
                continue;

            var cell = State.board.GetCell(piece.boardPos);

            if (piece.camp == Camp.Player)
            {
                playerHasPiece = true;
                if (cell != null && cell.isEnemyBase)
                    playerReachedEnemyBase = true;
            }
            else if (piece.camp == Camp.Enemy)
            {
                enemyHasPiece = true;
                if (cell != null && cell.isPlayerBase)
                    enemyReachedPlayerBase = true;
            }
        }

        return playerReachedEnemyBase || enemyReachedPlayerBase || !playerHasPiece || !enemyHasPiece;
    }

    private void EnterPlayerInputPhase()
    {
        State.phase = BattlePhase.WaitingInput;
        RefreshAvailableHighlights();
    }

    private void EnterResolvingPhase()
    {
        State.phase = BattlePhase.Resolving;
        ClearAvailableHighlights();
    }

    private void RefreshAvailableHighlights()
    {
        _highlightController?.RefreshHighlights(State);
    }

    private void ClearAvailableHighlights()
    {
        _highlightController?.ResetAvailabilityFlags(State);
        _highlightController?.ClearAllHighlights();
    }

    private void RebuildActionRestrictions()
    {
        State.playerRestrictions.Clear();
        State.enemyRestrictions.Clear();

        RebuildRestrictionsFromAuras();
        RebuildRestrictionsFromTemporaryEffects();
        RebuildRestrictionsFromExtraActionContext();
    }

    private void RebuildRestrictionsFromAuras()
    {
        // TODO: 由场上光环效果重建限制
    }

    private void RebuildRestrictionsFromTemporaryEffects()
    {
        // TODO: 由临时回合效果重建限制
    }

    private void RebuildRestrictionsFromExtraActionContext()
    {
        var ctx = State.extraActionContext;
        if (ctx == null || ctx.extraActionCount <= 0)
            return;

        CampActionRestrictionState currentRestriction = State.GetRestrictions(State.currentCamp);

        if (ctx.preferredPieceRuntimeId >= 0)
        {
            currentRestriction.allowedPieceRuntimeIds.Add(ctx.preferredPieceRuntimeId);
            currentRestriction.disableAllCards = true;
        }
    }

    private bool ShouldConsumeExistingExtraAction(BattleAction action)
    {
        var ctx = State.extraActionContext;
        if (ctx == null || ctx.extraActionCount <= 0)
            return false;

        if (action == null)
            return false;

        if (ctx.preferredPieceRuntimeId < 0)
            return true;

        return action.actorRuntimeId == ctx.preferredPieceRuntimeId;
    }

    private void ConsumeAction(bool consumedExistingExtraAction)
    {
        State.actionsRemaining = Mathf.Max(0, State.actionsRemaining - 1);

        if (!consumedExistingExtraAction)
            return;

        var ctx = State.extraActionContext;
        if (ctx == null || ctx.extraActionCount <= 0)
            return;

        ctx.extraActionCount--;
        if (ctx.extraActionCount <= 0)
        {
            ctx.Clear();
        }
    }

    private void ValidateExtraActionContext()
    {
        var ctx = State.extraActionContext;
        if (ctx == null || ctx.extraActionCount <= 0)
            return;

        if (ctx.preferredPieceRuntimeId < 0)
            return;

        var piece = State.GetPiece(ctx.preferredPieceRuntimeId);
        bool canStillAct = _availabilityService.CanSpecificPieceActIgnoringTurnEntryHighlight(State, piece);

        if (!canStillAct)
        {
            ctx.Clear();
        }
    }

    private bool ResolvePostActionAvailability()
    {
        RebuildActionRestrictions();
        ValidateExtraActionContext();
        RebuildActionRestrictions();

        return _availabilityService.HasAnyLegalAction(State, State.currentCamp);
    }

    private void AdvanceTurnOrInput()
    {
        bool hasLegalAction = ResolvePostActionAvailability();

        if (!hasLegalAction)
        {
            EndTurn();
            return;
        }

        if (State.currentCamp == Camp.Player)
        {
            EnterPlayerInputPhase();
        }
        else
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }
}
