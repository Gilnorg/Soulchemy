using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect {

    protected GameController gc;

    public string name;

    public int dmg, timer, range, deadRange;

    public GameObject visEffect;

    protected StatusEffect(int newDmg = 0, int newTimer = 3, int newRange = 0, int newDeadRange = 0)
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        dmg = newDmg;
        timer = newTimer;

        range = newRange;
        deadRange = newDeadRange;

        visEffect = gc.defaultVisEffect;
    }

    public StatusEffect(StatusEffect clone)
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        name = clone.name;

        dmg = clone.dmg;
        timer = clone.timer;

        range = clone.range;
        deadRange = clone.deadRange;

        visEffect = clone.visEffect;
    }

    public virtual void StartEffect(Entity target, int dmg)
    {

    }

    public virtual void EndEffect(Entity target, int dmg)
    {

    }

    public virtual void OnApply(Entity target, int dmg)
    {

    }

    public virtual void OnRemove(Entity target, int dmg)
    {

    }

}

public class Poison : StatusEffect
{
    public Poison(int newDmg = 3, int newTimer = 3, int newRange = 0, int newDeadRange = 0) 
        : base(newDmg, newTimer, newRange, newDeadRange)
    {
        name = "Poison";
    }

    public Poison(Poison newPoison)
        : base(newPoison)
    {
        name = "Poison";
    }

    public override void EndEffect(Entity target, int dmg)
    {
        target.Hurt(dmg);
    }
}
