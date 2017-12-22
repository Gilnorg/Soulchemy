using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : AIController {

    public AttackContainer SlimeAttack;

	// Use this for initialization
	new void Awake () {
        base.Awake();

        SlimeAttack.func = SlimeAttackFunc;
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
	}

    public override void Move()
    {
        //decide target
        currentTarget = CheckPriorities();

        //move to target
        MoveToEntity(currentTarget);

        if (IsAdjacent(currentTarget))
        {
            SetAttack(PickRandomAttack(BasicAttack, SlimeAttack));
        }
        else
        {
            List<Entity> adjacent = GetAdjacent();

            if (adjacent.Count > 0)
            {
                currentTarget = adjacent[Random.Range(0, adjacent.Count - 1)];
                SetAttack(PickRandomAttack(BasicAttack, SlimeAttack));
            }
            else
            {
                Advance();
                return;
            }
        }

        FaceMe(currentTarget);

        PlayAttack();
    }

    public void SlimeAttackFunc()
    {
        gc.currentBattle.SplashAttack(loc, atk.current, 1, 1);
    }
}
