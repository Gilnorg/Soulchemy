using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGController : AIController {

    public AttackContainer SplashHeal, SummonTotem;

    public float minHPPercent;

	new void Awake ()
    {
        base.Awake();

        SplashHeal.func = SplashHealFunc;
        SummonTotem.func = SummonTotemFunc;
    }

	new void Update ()
    {
        base.Update();
	}

    public override void Move()
    {
        bool canHeal = AdjacentHPPercent() <= minHPPercent;

        bool canTotem = gc.currentBattle.UnitCount < 6;

        List<AttackContainer> potentialAttacks = new List<AttackContainer> { BasicAttack };

        if (canHeal) potentialAttacks.Add(SplashHeal);
        if (canTotem) potentialAttacks.Add(SummonTotem);

        SetAttack(PickRandomAttack(potentialAttacks));

        currentTarget = CheckPriorities();

        FaceTarget(currentTarget);
        FaceMe(currentTarget);

        PlayAttack();
    }

    float AdjacentHPPercent()
    {
        float percent = 0, checkNum = 0;

        if (loc > 0)
        {
            var hp = gc.currentBattle.arena[loc - 1].hp;

            percent += hp.current / hp.max;
            checkNum++;
        }

        if (loc < gc.currentBattle.UnitCount - 1)
        {
            var hp = gc.currentBattle.arena[loc + 1].hp;

            percent += hp.current / hp.max;
            checkNum++;
        }

        return percent / checkNum;
    }

    public void SummonTotemFunc()
    {
        print("TODO");
    }

    public void SplashHealFunc()
    {
        print("TODO");
    }
}
