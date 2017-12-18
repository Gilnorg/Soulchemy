using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {
    
    [Range(0, 1)]
    public float playerPriority, companionPriority;

    protected Entity CheckPriorities()
    {
        Battle currentBattle = gc.currentBattle;

        Entity target = currentBattle.player;

        if (currentBattle.companion != null)
        {
            float chance = Random.Range(0f, 1f);

            if (playerPriority > companionPriority)
            {
                if (chance < companionPriority)
                {
                    target = currentBattle.companion;
                }
            }
            else if (playerPriority < companionPriority)
            {
                if (chance < playerPriority)
                {
                    target = currentBattle.player;
                }
            }
            else
            {
                if (chance > 0.5)
                {
                    target = currentBattle.player;
                }
                else
                {
                    target = currentBattle.companion;
                }
            }
        }

        return target;
    }

    protected void MoveToEntity(Entity target)
    {
        int dist = gc.currentBattle.currentEntity.loc - target.loc;
        int dir = Mathf.Abs(dist) / dist;

        if (dir > 0)
        {
            spRenderer.flipX = true;
        }
        else
        {
            spRenderer.flipX = false;
        }

        int lastLoc = gc.currentBattle.currentEntity.loc;
        gc.currentBattle.Mov(dist - dir);
        gc.currentBattle.currentEntity.mov.current -= Mathf.Abs(lastLoc - gc.currentBattle.currentEntity.loc);
    }

}
