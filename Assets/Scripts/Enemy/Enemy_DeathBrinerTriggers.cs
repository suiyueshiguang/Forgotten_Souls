using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DeathBrinerTriggers : Enemy_AnimationTriggers
{
    private Enemy_DeathBringer enemyDeathBriner => GetComponentInParent<Enemy_DeathBringer>();

    private void Relocate() => enemyDeathBriner.FindPosition();

    private void MakeInvisible() => enemyDeathBriner.fx.MakeTransprent(true);

    private void MakeVisible() => enemyDeathBriner.fx.MakeTransprent(false);

}
