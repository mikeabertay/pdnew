using System.Collections.Generic;
using System.Linq;

public class ActionAvailabilityService
{
    private readonly ActionCandidateBuilder _candidateBuilder = new();

    public bool HasAnyLegalAction(BattleState state, Camp camp)
    {
        if (state == null) return false;

        if (HasAnyLegalPieceAction(state, camp))
            return true;

        if (HasAnyLegalCardAction(state, camp))
            return true;

        return false;
    }

    public bool HasAnyLegalPieceAction(BattleState state, Camp camp)
    {
        foreach (var kv in state.pieces)
        {
            var piece = kv.Value;
            if (piece == null || piece.isDead || piece.camp != camp)
                continue;

            if (CanPieceAct(state, piece))
                return true;
        }

        return false;
    }

    public bool HasAnyLegalCardAction(BattleState state, Camp camp)
    {
        if (camp != Camp.Player)
            return false;

        if (state?.player?.hand?.cards == null)
            return false;

        foreach (var card in state.player.hand.cards)
        {
            if (CanCardUse(state, card))
                return true;
        }

        return false;
    }

    public bool CanPieceAct(BattleState state, PieceRuntime piece)
    {
        if (state == null || piece == null || piece.isDead)
            return false;

        if (piece.camp != state.currentCamp)
            return false;

        if (state.actionsRemaining <= 0)
            return false;

        if (!PassPieceRestriction(state, piece))
            return false;

        List<BattleAction> actions = _candidateBuilder.BuildForPiece(state, piece);
        actions = FilterPieceActionsByRestrictions(state, piece, actions);

        return actions.Count > 0;
    }

    public bool CanCardUse(BattleState state, CardRuntime card)
    {
        if (state == null || card == null || card.config == null)
            return false;

        if (state.currentCamp != Camp.Player)
            return false;

        if (state.actionsRemaining <= 0)
            return false;

        if (!PassCardRestriction(state, card))
            return false;

        List<BattleAction> actions = _candidateBuilder.BuildForCard(state, card);
        actions = FilterCardActionsByRestrictions(state, card, actions);

        return actions.Count > 0;
    }

    public List<PieceRuntime> GetAvailablePieces(BattleState state)
    {
        var result = new List<PieceRuntime>();
        if (state == null) return result;

        foreach (var kv in state.pieces)
        {
            var piece = kv.Value;
            if (CanPieceAct(state, piece))
            {
                result.Add(piece);
            }
        }

        return result;
    }

    public List<CardRuntime> GetAvailableCards(BattleState state)
    {
        var result = new List<CardRuntime>();
        if (state?.player?.hand?.cards == null) return result;

        foreach (var card in state.player.hand.cards)
        {
            if (CanCardUse(state, card))
            {
                result.Add(card);
            }
        }

        return result;
    }

    public bool CanSpecificPieceActIgnoringTurnEntryHighlight(BattleState state, PieceRuntime piece)
    {
        if (state == null || piece == null || piece.isDead)
            return false;

        if (piece.camp != state.currentCamp)
            return false;

        if (state.actionsRemaining <= 0)
            return false;

        if (!PassPieceRestriction(state, piece))
            return false;

        List<BattleAction> actions = _candidateBuilder.BuildForPiece(state, piece);
        actions = FilterPieceActionsByRestrictions(state, piece, actions);

        return actions.Count > 0;
    }

    private bool PassPieceRestriction(BattleState state, PieceRuntime piece)
    {
        var restriction = state.GetRestrictions(piece.camp);
        if (restriction == null)
            return true;

        if (restriction.disableAllPieces)
            return false;

        if (restriction.allowedPieceRuntimeIds.Count > 0 && !restriction.allowedPieceRuntimeIds.Contains(piece.runtimeId))
            return false;

        if (restriction.forbiddenPieceRuntimeIds.Contains(piece.runtimeId))
            return false;

        if (piece.displayConfig?.tags != null)
        {
            foreach (var tag in piece.displayConfig.tags)
            {
                if (restriction.forbiddenPieceTags.Contains(tag))
                    return false;
            }
        }

        return true;
    }

    private bool PassCardRestriction(BattleState state, CardRuntime card)
    {
        var restriction = state.GetRestrictions(Camp.Player);
        if (restriction == null)
            return true;

        if (restriction.disableAllCards)
            return false;

        if (restriction.allowedCardRuntimeIds.Count > 0 && !restriction.allowedCardRuntimeIds.Contains(card.runtimeId))
            return false;

        if (restriction.forbiddenCardRuntimeIds.Contains(card.runtimeId))
            return false;

        if (card.config.tags != null)
        {
            foreach (var tag in card.config.tags)
            {
                if (restriction.forbiddenCardTags.Contains(tag))
                    return false;
            }
        }

        return true;
    }

    private List<BattleAction> FilterPieceActionsByRestrictions(BattleState state, PieceRuntime piece, List<BattleAction> actions)
    {
        if (actions == null)
            return new List<BattleAction>();

        return actions.Where(a => a != null && a.CanExecute(state)).ToList();
    }

    private List<BattleAction> FilterCardActionsByRestrictions(BattleState state, CardRuntime card, List<BattleAction> actions)
    {
        if (actions == null)
            return new List<BattleAction>();

        return actions.Where(a => a != null && a.CanExecute(state)).ToList();
    }
}
