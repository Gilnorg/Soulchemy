using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : Enemy {

    private bool skoodle = true;

	// Use this for initialization
	new void Awake () {
        base.Awake();
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
            SetAttack(SlimeAttack, "Slime");
        }
        else
        {
            Advance();
            return;
        }

        PlayAttack();
    }

    public void BasicAttack()
    {
        currentTarget.Hurt(atk.current);
    }

    public void SlimeAttack()
    {
        gc.currentBattle.SplashAttack(loc, atk.current, 1, 1);
    }
}
