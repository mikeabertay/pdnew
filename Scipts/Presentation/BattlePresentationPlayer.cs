using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePresentationPlayer : MonoBehaviour
{
    public float speed = 1f;

    public IEnumerator Play(List<ResolutionStep> steps)
    {
        foreach (var step in steps)
        {
            yield return PlayStep(step);
        }
    }

    private IEnumerator PlayStep(ResolutionStep step)
    {
        switch (step)
        {
            case MoveStep move:
                yield return PlayMove(move);
                break;
            case KillStep kill:
                yield return PlayKill(kill);
                break;
            case SpawnStep spawn:
                yield return PlaySpawn(spawn);
                break;
            case UpgradeStep upgrade:
                yield return PlayUpgrade(upgrade);
                break;
        }
    }

    private IEnumerator PlayMove(MoveStep step)
    {
        // 这里接 PieceView 动画
        yield return new WaitForSeconds(0.15f / speed);
    }

    private IEnumerator PlayKill(KillStep step)
    {
        yield return new WaitForSeconds(0.15f / speed);
    }

    private IEnumerator PlaySpawn(SpawnStep step)
    {
        yield return new WaitForSeconds(0.15f / speed);
    }

    private IEnumerator PlayUpgrade(UpgradeStep step)
    {
        yield return new WaitForSeconds(0.15f / speed);
    }
}