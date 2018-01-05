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

        foreach (Entity entity in gc.currentBattle.arena)
        {
            if (entity.alliance != alliance)
            {
                targets.Add(entity);
            }
        }

        // TODO: Target Priorities
        return targets[Random.Range(0, targets.Count - 1)];
    }

    protected void MoveToEntity(Entity target)
    {
        int dist = loc - target.loc;
        int dir = (int)Mathf.Sign(dist);

        print(-(dist - dir));

        gc.currentBattle.Mov(-(dist - dir));
    }

    protected List<AttackContainer> lastAtk = new List<AttackContainer>();

    protected AttackContainer PickRandomAttack(params AttackContainer[] attacks)
    {
        return PickRandomAttack(attacks);
    }

    protected AttackContainer PickRandomAttack(List<AttackContainer> attacks)
    {
        int chance = Random.Range(0, 100);

        float lowestPriority = 100;
        AttackContainer chosenAttack = attacks[0];

        for (int i = 0; i < attacks.Count; i++)
        {
            var attack = attacks[i];

            for (int j = 0; j < lastAtk.Count; j++)
            {
                if (lastAtk[j].func == attack.func)
                {
                    attack.priority *= atkVarietyPercent + (1 - atkVarietyPercent) * j / lastAtk.Count;
                    break;
                }
            }

            if (attack.priority > chance)
            {
                if (attack.priority < lowestPriority)
                {
                    lowestPriority = attack.priority;
                    chosenAttack = attack;
                }
                else if (attack.priority == lowestPriority)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        chosenAttack = attack;
                    }
                }
            }
        }

        lastAtk.Insert(0, chosenAttack);
        if (lastAtk.Count > atkVariety)
        {
            lastAtk.RemoveAt(lastAtk.Count - 1);
        }

        return chosenAttack;
    }

}
