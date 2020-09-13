using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;

public abstract class SplitAgentBase : BaseTankAgent
{
  
    public MultiBrainAgent mAgent;

    protected override void OnInitialize()
    {
        mAgent = GetComponentInParent<MultiBrainAgent>();
        area = mAgent.area;
        tank = mAgent.tank;
        if (tank == null)
            tank = GetComponentInParent<TankController>();
    }

    protected abstract void HandleRewards();

    public override void OnEnergyRecharge()
    {

    }
    public override void OnHit(DamagableTarget hitObject, float damage)
    {

    }

    public override void OnFire()
    {
    }
}
