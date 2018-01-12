using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGController : AIController {

    public AttackContainer SplashHeal, SummonTotem;

    public GameObject totem;

    public float minHPPercent;
    public int healStrength;

    private Entity healTarget;

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
        List<AttackContainer> potentialAttacks = new List<AttackContainer> ();
        
        currentTarget = CheckPriorities();

        healTarget = CheckHPPercentPairs();

        if (IsInRange(currentTarget, mov.current))
        {
            potentialAttacks.Add(BasicAttack);
        }

        if (healTarget != null)
        {
            potentialAttacks.Add(SplashHeal);
        }

        if (gc.battle.UnitCount < 6 && loc != 0 && loc != gc.battle.UnitCount - 1)
        {
            potentialAttacks.Add(SummonTotem);
        }

        SetAttack(PickRandomAttack(potentialAttacks));

        if (currentAttack == BasicAttack.func)
        {
            MoveToEntity(currentTarget);
            if (!IsInRange(currentTarget))
            {
                Advance();
                return;
            }

            FaceTarget(currentTarget);
            FaceMe(currentTarget);
        }
        else if (currentAttack == SplashHeal.func)
        {
            currentTarget = healTarget;

            MoveToEntity(currentTarget);
            if (!IsInRange(currentTarget))
            {
                Advance();
                return;
            }

            FaceMe(currentTarget);
            FaceMe(gc.battle.arena[loc + 1]);
        }

        PlayAttack();
    }

    Entity CheckHPPercentPairs()
    {
        List<Entity> potentialPairs = new List<Entity>();

        for (int i = Mathf.Clamp(loc - mov.current, 1, gc.battle.UnitCount); i < Mathf.Clamp(loc + mov.current, 1, gc.battle.UnitCount - 1); i++)
        {
            Entity unit1 = gc.battle.arena[i], unit2 = gc.battle.arena[i - 1];

            float percent = 0;

            percent += unit1.hp.current / unit1.hp.max;
            percent += unit2.hp.current / unit2.hp.max;
            percent /= 2;

            if (percent <= minHPPercent)
            {
                potentialPairs.Add(gc.battle.arena[i - 1]);
            }
        }

        if (potentialPairs.Count > 0)
        {
            return potentialPairs[Random.Range(0, potentialPairs.Count - 1)];
        }
        else
        {
            return null;
        }
    }

    public void SummonTotemFunc()
    {
        float chance = Random.Range(0f, 1f);

        if (chance < 0.5f)
        {
            gc.battle.SpawnEntity(totem, loc);
            spRenderer.flipX = true;
        }
        else
        {
            gc.battle.SpawnEntity(totem, loc + 1);
            spRenderer.flipX = false;
        }
    }

    public void SplashHealFunc()
    {
        gc.battle.SplashHeal(loc, healStrength);
    }
}
