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
        startPos = loc;

        gc.currentBattle.state = BattleState.playerTurn;
    }

    public override void Hurt(int dmg, string dmgType, Entity attacker = null)
    {
        if (!gc.cheatInvincible)
        {
            base.Hurt(dmg, dmgType, attacker);
            if (hp.current <= 0)
            {
                gc.currentBattle.Lose();
            }
        }
    }

    public void PlayAttack(Entity newTarget)
    {
        gc.currentBattle.state = BattleState.enemyTurn;

        currentTarget = newTarget;

        FaceMe(currentTarget);

        PlayAttack();
    }

    //BUTTON TRIGGERS
    public void AttackButton()
    {
        if (currentAttack != HurtTarget)
        {
            SetAttack(HurtTarget, "Attack");
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

    int startPos;

    public void MOVLeft()
    {
        if (loc - 1 >= startPos)
        {
            mov.current += 2;
        }

        gc.currentBattle.MovLeft();
        gc.currentBattle.AttackPreview(this);

        spRenderer.flipX = true;
    }

    public void MOVRight()
    {
        if (loc + 1 <= startPos)
        {
            mov.current += 2;
        }

        gc.currentBattle.MovRight();
        gc.currentBattle.AttackPreview(this);

        spRenderer.flipX = false;
    }

}
