using System.Collections.Generic;

public class CardPile
{
    public List<CardRuntime> cards = new();

    public void Add(CardRuntime card) => cards.Add(card);

    public CardRuntime DrawTop()
    {
        if (cards.Count == 0) return null;
        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}

public class PlayerBattleState
{
    public Camp camp;
    public CardPile drawPile = new();
    public CardPile hand = new();
    public CardPile discardPile = new();
    public CardPile exhaustedPile = new();

    public int handLimit = 8;
    public int populationLimit = 10;
}

public class TriggerEvent
{
    public TriggerType triggerType;
    public int sourceRuntimeId;
    public int targetRuntimeId;
}

public class BattleState
{
    public BoardState board;
    public PlayerBattleState player;
    public PlayerBattleState enemy;

    public Dictionary<int, PieceRuntime> pieces = new();

    public Camp currentCamp;
    public int turnIndex;
    public int actionsRemaining;
    public BattlePhase phase;

    public Queue<TriggerEvent> triggerQueue = new();

    public CampActionRestrictionState playerRestrictions = new() { targetCamp = Camp.Player };
    public CampActionRestrictionState enemyRestrictions = new() { targetCamp = Camp.Enemy };

    public ExtraActionContext extraActionContext = new();

    public PieceRuntime GetPiece(int runtimeId)
    {
        pieces.TryGetValue(runtimeId, out var piece);
        return piece;
    }

    public CampActionRestrictionState GetRestrictions(Camp camp)
    {
        return camp == Camp.Player ? playerRestrictions : enemyRestrictions;
    }
}