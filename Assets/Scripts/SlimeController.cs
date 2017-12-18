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

        SetAttack(BasicAttack, "Attack");

        PlayAttack();
    }

    public void BasicAttack()
    {
        print("Attacks!");
    }

    public void SlimeAttack()
    {
        print("SlimeAttacks!");
    }

    protected void UsePoison(Entity target, int dmg)
    {
        gc.currentBattle.SplashEffect(target, new Poison());
    }
}
