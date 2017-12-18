using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

    public string name;
    public string visEffect;
    public Combat.Effect effect;
    public float dmg;
    public int range, deadRange;
    public bool isMelee = false;
    public int priority = 0;

    public Attack(string newName, string newVisEffect, Combat.Effect newEffect, int newRange)
    {
        setStats(newName, newVisEffect, newEffect, newRange, 0, false, 0);
    }
    public Attack(string newName, string newVisEffect, Combat.Effect newEffect, int newRange, bool newIsMelee)
    {
        setStats(newName, newVisEffect, newEffect, newRange, 0, newIsMelee, 0);
    }
    public Attack(string newName, string newVisEffect, Combat.Effect newEffect, int newRange, int newDeadRange)
    {
        setStats(newName, newVisEffect, newEffect, newRange, newDeadRange, false, 0);
    }
    public Attack(string newName, string newVisEffect, Combat.Effect newEffect, int newRange, int newDeadRange, bool newIsMelee)
    {
        setStats(newName, newVisEffect, newEffect, newRange, newDeadRange, newIsMelee, 0);
    }
    public Attack(string newName, string newVisEffect, Combat.Effect newEffect, int newRange, int newDeadRange, int newPriority)
    {
        setStats(newName, newVisEffect, newEffect, newRange, newDeadRange, false, newPriority);
    }
    public Attack(Attack newAttack)
    {
        setStats(newAttack.name, newAttack.visEffect, newAttack.effect, newAttack.range, newAttack.deadRange, newAttack.isMelee, newAttack.priority);
    }
    public Attack(Attack newAttack, int newPriority)
    {
        setStats(newAttack.name, newAttack.visEffect, newAttack.effect, newAttack.range, newAttack.deadRange, newAttack.isMelee, newPriority);
    }

    private void setStats(string newName, string newVisEffect, Combat.Effect newEffect, int newRange, int newDeadRange, bool newIsMelee, int newPriority)
    {
        name = newName;
        visEffect = newVisEffect;
        effect = newEffect;
        range = newRange;
        deadRange = newDeadRange;
        isMelee = newIsMelee;

        if (newPriority > 100) priority = 100;
        else if (newPriority < 0) priority = 0;
        else priority = newPriority;
    }

}
