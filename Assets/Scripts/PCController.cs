using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCController : Entity {

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
        base.Move();

        gc.currentBattle.state = BattleState.playerTurn;
    }

    public void PlayAttack(Entity newTarget)
    {
        gc.currentBattle.state = BattleState.enemyTurn;

        currentTarget = newTarget;

        FaceMe(currentTarget);

        PlayAttack();
    }

    private void BasicAttack()
    {
        print(currentTarget.name + currentTarget.id);
    }


    public void AttackButton()
    {
        if (currentAttack != BasicAttack)
        {
            SetAttack(BasicAttack, "Attack");
            gc.currentBattle.currentPlayerAttack = new PlayerAttack(true);

            gc.currentBattle.AttackPreview(this);
        }
        else
        {
            NullAttack();

            gc.currentBattle.NullCurrentAttackPreview();
            gc.currentBattle.AttackPreview();
        }
    }
    

}
