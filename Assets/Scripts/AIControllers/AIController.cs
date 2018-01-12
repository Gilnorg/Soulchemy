using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Entity
{
    public int atkVariety;
    public float atkVarietyPercent;

    [System.Serializable]
    public struct AttackContainer
    {
        [Range(0, 100)]
        public float priority;

        public Empty func;
        public string anim;
    }

    public AttackContainer BasicAttack;

    protected new void Awake()
    {
        base.Awake();

        //Attacks
        BasicAttack.anim = "Attack";
        BasicAttack.func = HurtTarget;
    }

    public void SetAttack(AttackContainer attack)
    {
        currentAttack = attack.func;
        currentAnim = attack.anim;
    }

    protected Entity CheckPriorities()
    {
        List<Entity> targets = new List<Entity>();

        foreach (Entity entity in gc.battle.arena)
        {
            if (entity.alliance != alliance && !entity.dead)
            {
                targets.Add(entity);
            }
        }

        // TODO: Target Priorities
        return targets[Random.Range(0, targets.Count - 1)];
    }

    protected void MoveTo(int newLoc)
    {
        int dist = loc - newLoc;
        int dir = (int)Mathf.Sign(dist);

        gc.battle.Mov(-(dist - dir));
    }

    protected void MoveToEntity(Entity target)
    {
        int dist = loc - target.loc;
        int dir = (int)Mathf.Sign(dist);

        gc.battle.Mov(-(dist - dir));
    }

    protected List<AttackContainer> lastAtk = new List<AttackContainer>();

    protected AttackContainer PickRandomAttack(params AttackContainer[] attacks)
    {
        List<AttackContainer> attackList = new List<AttackContainer>();

        foreach(AttackContainer attack in attacks)
        {
            attackList.Add(attack);
        }

        return PickRandomAttack(attackList);
    }

    protected AttackContainer PickRandomAttack(List<AttackContainer> attacks)
    {
        for (int i = 0; i < 30; i++)
        {
            var attack = attacks[Random.Range(0, attacks.Count)];

            for (int j = 0; j < lastAtk.Count; j++)
            {
                if (lastAtk[j].func == attack.func)
                {
                    attack.priority *= atkVarietyPercent + (1f - atkVarietyPercent) * ((float)j / lastAtk.Count);
                    break;
                }
            }
            

            if (attack.priority >= Random.Range(0, 100))
            {
                return attack;
            }
        }
        
        return attacks[Random.Range(0, attacks.Count)];
    }

}
