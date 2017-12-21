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

        FaceMe(currentTarget);

        if (IsAdjacent(currentTarget))
        {
            SetAttack(PickRandomAttack(BasicAttack, SlimeAttack));
        }
        else
        {
            Advance();
            return;
        }

        PlayAttack();
    }

    public void SlimeAttackFunc()
    {
        gc.currentBattle.SplashAttack(loc, atk.current, 1, 1);
    }
}
