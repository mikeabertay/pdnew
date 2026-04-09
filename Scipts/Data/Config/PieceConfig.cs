using System;
using System.Collections.Generic;

[Serializable]
public class CardConfig
{
    public string cardId;
    public string cardName;
    public string cardType; // Piece / Spell / Hero
    public string pieceId;
    public int weight;
    public bool exclusive;
    public bool temporary;
    public List<string> tags = new();
    public List<EffectConfig> onPlayEffects = new();
    public List<TargetRuleConfig> targetRules = new();
}

[Serializable]
public class PieceConfig
{
    public string pieceId;
    public string pieceName;
    public int level;
    public bool isHero;
    public List<OffsetConfig> moveOffsets = new();
    public List<string> tags = new();
    public List<EffectConfig> passiveEffects = new();
    public List<EffectConfig> onSpawnEffects = new();
    public List<EffectConfig> onUpgradeEffects = new();
    public UpgradeRuleConfig upgradeRule;
}

[Serializable]
public class EffectConfig
{
    public string effectType;
    public List<string> parameters = new();
}

[Serializable]
public class OffsetConfig
{
    public int x;
    public int y;
}

[Serializable]
public class UpgradeRuleConfig
{
    public string ruleType; // Normal / ExtraCondition / OnlyCondition
    public List<string> parameters = new();
}

[Serializable]
public class TargetRuleConfig
{
    public string ruleType;
    public List<string> parameters = new();
}