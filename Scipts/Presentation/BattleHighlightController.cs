using System.Collections.Generic;
using UnityEngine;

public class BattleHighlightController : MonoBehaviour
{
    private readonly ActionAvailabilityService _availabilityService = new();

    public void RefreshHighlights(BattleState state)
    {
        ResetAvailabilityFlags(state);
        ClearAllHighlights();

        if (state == null)
            return;

        if (state.currentCamp != Camp.Player)
            return;

        if (state.phase != BattlePhase.WaitingInput && state.phase != BattlePhase.TurnStart)
            return;

        List<PieceRuntime> availablePieces = _availabilityService.GetAvailablePieces(state);
        List<CardRuntime> availableCards = _availabilityService.GetAvailableCards(state);

        foreach (var piece in availablePieces)
        {
            piece.isAvailable = true;
            HighlightPiece(piece, true);
        }

        foreach (var card in availableCards)
        {
            card.isAvailable = true;
            HighlightCard(card, true);
        }
    }

    public void ClearAllHighlights()
    {
        // TODO:
        // 1. 清棋子高亮
        // 2. 清手牌高亮
        // 3. 清目标格高亮
        // 4. 清临时选择高亮
    }

    public void ResetAvailabilityFlags(BattleState state)
    {
        if (state == null) return;

        foreach (var kv in state.pieces)
        {
            if (kv.Value != null)
                kv.Value.isAvailable = false;
        }

        if (state.player?.hand?.cards != null)
        {
            foreach (var card in state.player.hand.cards)
            {
                if (card != null)
                    card.isAvailable = false;
            }
        }
    }

    private void HighlightPiece(PieceRuntime piece, bool active)
    {
        // TODO: 接 PieceView
    }

    private void HighlightCard(CardRuntime card, bool active)
    {
        // TODO: 接 CardView
    }
}