using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect {

    public string name;
    public Combat.Effect startEffect, endEffect;
    public float dmg;
    public int timeLeft;

    public StatusEffect(string newName, Combat.Effect newStartEffect, Combat.Effect newEndEffect, int newTimeLeft)
    {
        name = newName;
        startEffect = newStartEffect;
        endEffect = newEndEffect;
        timeLeft = newTimeLeft;
    }

    public StatusEffect(StatusEffect clone)
    {
        name = clone.name;
        startEffect = clone.startEffect;
        dmg = clone.dmg;
        timeLeft = clone.timeLeft;
    }

}
