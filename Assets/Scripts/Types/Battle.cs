using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { start, enemyTurn, playerTurn, win, loss }

public class PlayerAttack
{
    public bool isMelee;

    public int range, deadRange;

    public PlayerAttack(bool newIsMelee = false, int newRange = 0, int newDeadRange = 0)
    {
        isMelee = newIsMelee;
        range = newRange;
        deadRange = newDeadRange;
    }
}

[System.Serializable]
public class Battle {
    public BattleState state = BattleState.start;

    public bool set = false;
    public bool triggered = false;
    public int currentTurn = 0;

    public Entity player, companion;

    public List<Entity> arena = new List<Entity>();

    private GameController gc;

    public PlayerAttack currentPlayerAttack = null;
    public Item currentItem = null;

    //GETTERS
    public int Count
    {
        get { return arena.Count; }
    }

    public Entity currentEntity
    {
        get { return arena[currentTurn]; }
    }


    //CONSTRUCTORS
    public Battle()
    {
        
    }

    public Battle(GameController newGC, List<GameObject> enemies)
    {
        set = true;

        gc = newGC;

        player = gc.player;
        companion = gc.companion;

        //copy enemies
        foreach (GameObject enemy in enemies)
        {
            var newEnemy = Object.Instantiate(enemy, gc.battleUnits.transform);
            newEnemy.name = enemy.name;
            newEnemy.transform.position = new Vector3(0, GameController.floorY);

            newEnemy.SetActive(false);
        }
    }


    //BATTLE FLOW
    public void Trigger()
    {
        triggered = true;

        foreach (Transform transform in gc.battleUnits.transform)
        {
            transform.gameObject.SetActive(true);

            //add entity to list
            Entity entity = transform.GetComponent<Entity>();
            arena.Add(entity);
            entity.loc = arena.Count - 1;
            if (entity.alliance != Alliance.friendly)
            {
                entity.spRenderer.flipX = true;
            }
        }

        gc.state = GameState.inBattle;

        Advance();
    }

    public void Advance()
    {
        state = BattleState.enemyTurn;

        //find entity with greatest spd
        int bigSpd = -100, newCurrentTurn = -1;
        bool won = true;

        foreach (Entity entity in arena)
        {
            if (!entity.dead && entity.alliance != Alliance.friendly)
            {
                won = false;
            }

            if (entity.spd.current > bigSpd && !entity.gone && !entity.dead)
            {
                bigSpd = entity.spd.current;
                newCurrentTurn = entity.loc;
            }
        }

        if (won)
        {
            Win();
            return;
        }

        //if new enemy has been found, move on
        if (newCurrentTurn != -1)
        {
            //end turn
            ApplyStatusEffects(false);

            currentEntity.mov.Reset();

            //start new turn
            currentTurn = newCurrentTurn;

            currentEntity.gone = true;

            ApplyStatusEffects(true);

            currentEntity.animator.Play("Bounce");
        }
        else //if all enemies have gone, restart
        {
            foreach (Entity entity in arena)
            {
                entity.gone = false;
            }

            Advance();
        }
    }

    public void Win()
    {
        gc.state = GameState.inField;

        foreach(Entity entity in arena)
        {
            if (entity.alliance != Alliance.friendly)
            {
                Object.Destroy(entity.gameObject);
                Object.Destroy(entity.healthBar);
                arena.Remove(entity);
                set = false;
                Win();
                break;
            }
            else
            {
                entity.gone = false;
            }
        }
    }

    public void Lose()
    {

    }


    //MOVE LEFT
    public void MovLeft(int dist = 1)
    {
        Mov(-dist);
    }
    public void MovLeft(Entity entity, int dist = 1)
    {
        Mov(entity, -dist);
    }

    //MOVE RIGHT
    public void MovRight(int dist = 1)
    {
        Mov(dist);
    }
    public void MovRight(Entity entity, int dist = 1)
    {
        Mov(entity, dist);
    }

    //MOVE ENTITY
    public void Mov(int dist)
    {
        Mov(currentEntity, dist);
    }

    public void Mov(Entity entity, int dist)
    {
        int dir = (int)Mathf.Sign(dist);
        
        dist = Mathf.Clamp(dist, -entity.loc, arena.Count - entity.loc - 1);

        if (dist == 0) return;

        for (int i = 0; Mathf.Abs(i) < Mathf.Abs(dist); i += dir)
        {
            Entity checkEntity = arena[entity.loc + dir];

            // if nothing is in the way...
            if (!checkEntity.blocking && !checkEntity.lockedLeft && !checkEntity.lockedRight)
            {
                if (entity.mov.current >= 1)
                    MovForwards(entity, dir);
                else return;
            }

            // if enemy is not blocking...
            else if (!checkEntity.blocking)
            {
                int lockStep = dir;

                // if entity is multitile...
                if (checkEntity.lockedLeft)
                {
                    while (!arena[checkEntity.loc + lockStep].lockedRight)
                    {
                        lockStep += dir;
                        if (checkEntity.loc + lockStep < 2 || i + lockStep > arena.Count - 2)
                            break;
                    }

                    lockStep += dir;

                    if (Mathf.Abs(lockStep) <= entity.mov.current)
                    {
                        MovForwards(entity, lockStep);
                        i += lockStep;
                    }
                    else return;
                }
                else if (checkEntity.lockedRight)
                {
                    while (!arena[checkEntity.loc + lockStep].lockedLeft)
                    {
                        lockStep += dir;
                        if (checkEntity.loc + lockStep < 2 || i + lockStep > arena.Count - 2)
                            break;
                    }

                    lockStep += dir;

                    if (Mathf.Abs(lockStep) <= entity.mov.current)
                    {
                        MovForwards(entity, lockStep);
                        i += lockStep;
                    }
                    else return;
                }
            }
        }
    }

    private void MovForwards(Entity entity, int dist)
    {
        arena.Remove(entity);

        if (entity.loc + dist < arena.Count)
        {
            arena.Insert(entity.loc + dist, entity);
        }
        else
        {
            arena.Add(entity);
        }

        SetLocations();
        currentTurn += dist;

        entity.mov.current -= Mathf.Abs(dist);
    }

    public void SetLocations()
    {
        for (int i = 0; i < Count; i++)
        {
            arena[i].loc = i;
        }
    }

    //ATTACK TOOLS
    public void SetCurrentAttackPreview(bool newIsMelee = false, int newRange = 0, int newDeadRange = 0)
    {
        currentPlayerAttack = new PlayerAttack(newIsMelee, newRange, newDeadRange);
        gc.currentItem = null;
    }
    public void SetCurrentAttackPreview(Item item)
    {
        currentPlayerAttack = new PlayerAttack(item.type == ItemType.Potion, item.range, item.deadRange);
        gc.currentItem = item;
    }

    public void AttackPreview()
    {
        foreach(Entity entity in arena)
        {
            entity.attackReticle.SetActive(false);
        }
    }

    public void NullCurrentAttackPreview()
    {
        currentPlayerAttack = null;
        gc.currentItem = null;
        AttackPreview();
    }

    public void AttackPreview(Entity target)
    {
        if (state == BattleState.playerTurn && currentPlayerAttack != null)
        {
            if (currentPlayerAttack.isMelee)
            {
                target = player;

                int LBounds = Mathf.Clamp(target.loc - 1, 0, Count);
                int UBounds = Mathf.Clamp(target.loc + 1, 0, Count);

                for (int i = 0; i < Count; i++)
                {
                    if (i >= LBounds && i <= UBounds
                        && (i <= target.loc - 1 || i >= target.loc + 1))
                    {
                        arena[i].attackReticle.SetActive(true);
                    }
                    else
                    {
                        arena[i].attackReticle.SetActive(false);
                    }
                }
            }
            else
            {
                int LBounds = Mathf.Clamp(target.loc - currentPlayerAttack.range, 0, Count);
                int UBounds = Mathf.Clamp(target.loc + currentPlayerAttack.range, 0, Count);

                for (int i = 0; i < Count; i++)
                {
                    if (i >= LBounds && i <= UBounds
                        && (i <= target.loc - currentPlayerAttack.deadRange || i >= target.loc + currentPlayerAttack.deadRange))
                    {
                        arena[i].attackReticle.SetActive(true);
                    }
                    else
                    {
                        arena[i].attackReticle.SetActive(false);
                    }
                }
            }
        }
    }

    private void ApplyStatusEffects(bool start)
    {
        List<StatusEffect> deadEffects = new List<StatusEffect>();

        if (start)
        {
            foreach (StatusEffect statusEffect in currentEntity.statusEffects)
            {
                statusEffect.StartEffect(currentEntity, statusEffect.dmg);
            }
        }
        else
        {
            foreach (StatusEffect statusEffect in currentEntity.statusEffects)
            {
                statusEffect.EndEffect(currentEntity, statusEffect.dmg);
                statusEffect.timer--;

                if (statusEffect.timer <= 0)
                {
                    deadEffects.Add(statusEffect);
                }
            }
        }

        foreach (StatusEffect deadEffect in deadEffects)
        {
            deadEffect.OnRemove(currentEntity, deadEffect.dmg);

            currentEntity.statusEffects.Remove(deadEffect);
        }
    }


    //Splash Attack
    public void SplashAttack(int target, int dmg, int range = 1, int deadRange = 0)
    {
        int LBounds = Mathf.Clamp(target - range, 0, Count - 1);
        int UBounds = Mathf.Clamp(target + range, 0, Count - 1);

        for (int i = LBounds; i <= UBounds; i++)
        {
            if (i <= target - deadRange || i >= target + deadRange)
            {
                arena[i].Hurt(dmg);
            }
        }
    }
    
    //Splash Effect
    public void SplashEffect(int target, StatusEffect statusEffect, int range = 1, int deadRange = 0)
    {
        int LBounds = Mathf.Clamp(target - range, 0, Count - 1);
        int UBounds = Mathf.Clamp(target + range, 0, Count - 1);

        for (int i = LBounds; i <= UBounds; i++)
        {
            if (i <= target - deadRange || i >= target + deadRange)
            {
                arena[i].statusEffects.Add(new StatusEffect(statusEffect));

                statusEffect.OnApply(arena[i], statusEffect.dmg);

                Object.Instantiate(statusEffect.visEffect, arena[i].transform);
            }
        }
    }

}
