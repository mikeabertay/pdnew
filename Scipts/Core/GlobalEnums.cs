public enum Camp
{
    None = 0,
    Player = 1,
    Enemy = 2
}

public enum BattlePhase
{
    None,
    BattleStart,
    TurnStart,
    WaitingInput,
    Resolving,
    Animating,
    TurnEnd,
    BattleEnd
}

public enum ActionType
{
    None,
    Move,
    Attack,
    PlayCard,
    Summon,
    Upgrade,
    SpecialMove,
    SpecialAttack,
    SpecialAction
}

public enum TriggerType
{
    BattleStart,
    TurnStart,
    TurnEnd,
    OnSpawn,
    OnUpgrade,
    OnMove,
    OnAttack,
    OnKill,
    OnDeath,
    OnBattleWin,
    OnBattleLose
}

public enum TerrainType
{
    Normal,
    Base,
    Special
}