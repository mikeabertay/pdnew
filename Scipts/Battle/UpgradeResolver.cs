public class UpgradeResolver
{
    public bool CanUpgrade(BattleState state, PieceRuntime targetPiece, CardRuntime materialCard)
    {
        if (targetPiece == null || materialCard?.config == null)
            return false;

        var pieceConfig = ConfigManager.Instance.GetPiece(materialCard.config.pieceId);
        if (pieceConfig == null)
            return false;

        // 这里只写最基本规则：lv+1
        return pieceConfig.level == targetPiece.CurrentLevel + 1;
    }

    public bool ExecuteUpgrade(BattleState state, PieceRuntime targetPiece, CardRuntime materialCard)
    {
        if (!CanUpgrade(state, targetPiece, materialCard))
            return false;

        var newPieceConfig = ConfigManager.Instance.GetPiece(materialCard.config.pieceId);

        targetPiece.layers.Add(new PieceLayer
        {
            sourceCardId = materialCard.config.cardId,
            sourceName = materialCard.config.cardName,
            pieceConfig = newPieceConfig,
            acquireOrder = materialCard.acquireOrder
        });

        targetPiece.displayConfig = newPieceConfig;

        state.triggerQueue.Enqueue(new TriggerEvent
        {
            triggerType = TriggerType.OnUpgrade,
            sourceRuntimeId = targetPiece.runtimeId
        });

        return true;
    }
}