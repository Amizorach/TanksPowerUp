using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableTank : DamagableTarget
{
    public TankController tank;

    public void Awake()
    {
        tank = GetComponent<TankController>();
    }
    public override float TakeDamage(float damage) {
        return tank.TakeDamage(damage);
    }

}

