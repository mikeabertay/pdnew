using System.Collections.Generic;

public class TriggerDispatcher
{
    public List<ResolutionStep> ResolveAll(BattleState state)
    {
        var result = new List<ResolutionStep>();

        while (state.triggerQueue.Count > 0)
        {
            var trigger = state.triggerQueue.Dequeue();
            ResolveSingleTrigger(state, trigger, result);
        }

        return result;
    }

    private void ResolveSingleTrigger(BattleState state, TriggerEvent trigger, List<ResolutionStep> result)
    {
        foreach (var kv in state.pieces)
        {
            var piece = kv.Value;
            if (piece == null || piece.isDead) continue;

            foreach (var layer in piece.layers)
            {
                if (layer.pieceConfig == null) continue;

                foreach (var effect in layer.pieceConfig.passiveEffects)
                {
                    if (CanRespond(trigger, effect))
                    {
                        ResolveEffect(state, piece, effect, result);
                    }
                }
            }

            foreach (var extra in piece.extraEffects)
            {
                if (CanRespond(trigger, extra.effectConfig))
                {
                    ResolveEffect(state, piece, extra.effectConfig, result);
                }
            }
        }
    }

    private bool CanRespond(TriggerEvent trigger, EffectConfig effect)
    {
        if (effect == null) return false;
        return effect.effectType == trigger.triggerType.ToString();
    }

    private void ResolveEffect(BattleState state, PieceRuntime owner, EffectConfig effect, List<ResolutionStep> result)
    {
        // 这里先放成分发口，后续接真正的 EffectResolver
    }
}