using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceRuntime
{
    public int runtimeId;
    public Camp camp;
    public Vector2Int boardPos;
    public bool isDead;
    public bool hasActedThisTurn;

    public PieceConfig displayConfig;
    public List<PieceLayer> layers = new();
    public List<GrantedEffectRuntime> extraEffects = new();
    public bool isAvailable;
    public int CurrentLevel
    {
        get
        {
            int max = 1;
            foreach (var layer in layers)
            {
                if (layer.pieceConfig != null && layer.pieceConfig.level > max)
                    max = layer.pieceConfig.level;
            }
            return max;
        }
    }

    public int PopulationCost => CurrentLevel;
}

public class PieceLayer
{
    public string sourceCardId;
    public string sourceName;
    public PieceConfig pieceConfig;
    public int acquireOrder; // 后发先至判断时可用
}

public class GrantedEffectRuntime
{
    public string sourceName;
    public EffectConfig effectConfig;
    public int acquireOrder;
}


public class CardRuntime
{
    public int runtimeId;
    public CardConfig config;
    public bool isTemporary;
    public bool exhausted; // 英雄牌这种用后不回弃牌
    public int acquireOrder;
    public bool isAvailable;
}

public class ExtraActionContext
{
    public int extraActionCount;

    // 优先/限定来源
    public int preferredPieceRuntimeId = -1;

    // 是否严格限定为该棋子
    public bool strictLockToPiece;

    public void Clear()
    {
        extraActionCount = 0;
        preferredPieceRuntimeId = -1;
        strictLockToPiece = false;
    }
}