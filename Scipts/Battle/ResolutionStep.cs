using UnityEngine;

public abstract class ResolutionStep
{
}

public class MoveStep : ResolutionStep
{
    public int pieceRuntimeId;
    public Vector2Int from;
    public Vector2Int to;
}

public class KillStep : ResolutionStep
{
    public int targetRuntimeId;
}

public class SpawnStep : ResolutionStep
{
    public int pieceRuntimeId;
    public Vector2Int pos;
}

public class UpgradeStep : ResolutionStep
{
    public int targetPieceRuntimeId;
    public string newPieceId;
}