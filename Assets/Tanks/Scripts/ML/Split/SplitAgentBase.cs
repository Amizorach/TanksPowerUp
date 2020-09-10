using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;

public abstract class SplitAgentBase : Agent
{
    public MLRewards rewards;
    protected bool dead = false;
    public TanksAreaBase area;
    public TankController tank;
    protected MLStats stats;
    public float[] actions;

    public MultiBrainAgent mAgent;

    //For debug
    public float reward;

    public override void Initialize()
    {
        Debug.Log("Initialize");
        mAgent = GetComponentInParent<MultiBrainAgent>();
        area = mAgent.area;
        tank = mAgent.tank;
        if (tank == null)
            tank = GetComponentInParent<TankController>();

        OnInitialize();
    }

    protected virtual void OnInitialize()
    {
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        if (stats != null)
            SendStats();
        reward = 0f;
        if (area.GetRewards() != null)
            rewards = area.GetRewards();
        stats = new MLStats(Academy.Instance.StatsRecorder);
        tank.OnEpisodeBegin(area.GetTankSettings());
        dead = false;
    }

    public void Win()
    {
        PostReward(rewards.agentRewards.winReward);
        Die();
    }

    public void Die()
    {
        dead = true;
        tank.Die();
        EndEpisode();

    }

   

    protected abstract void HandleRewards();

  


    //public override void CollectObservations(VectorSensor sensor)
    //{
    //    invVel = this.transform.InverseTransformVector(tank.GetVelocity());
    //    sensor.AddObservation(invVel); // vec 3
    //    sensor.AddObservation(tank.energy);

    //    //foreach (YTFlag flag in area.flags)
    //    //{
    //    //    AddTeamedObservation(sensor, flag.transform, flag.teamId);
    //    //}

    //    HandleDriverRewards();
    //}

    public void AddObservationForTransform(VectorSensor sensor, Transform target)
    {
        //4 for flag
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        sensor.AddObservation(
               transform.InverseTransformDirection(dirToTarget)); // vec 3
        Vector3 invPos = transform.InverseTransformPoint(transform.position);
        sensor.AddObservation(invPos.x); // vec 3
        sensor.AddObservation(invPos.z); // vec 3
    }

    public void AddTeamedObservation(VectorSensor sensor, Transform target, int team)
    {
        AddObservationForTransform(sensor, target);
        sensor.AddObservation(mAgent.teamId == team);
    }

    public void SendStats()
    {
        stats.totalDistance = tank.GetTotalDistance();
        stats.Send();
    }


    public void PostReward(float r)
    {
        reward += r;
        AddReward(r);
    }

    internal virtual void OnEnergyRecharge()
    {

    }
    internal virtual void OnHit(DamagableTarget hitObject, float damage)
    {

    }

    internal virtual void OnFire()
    {
    }
}
//[System.Serializable]

//public class TankDriverRewards
//{
//    public float collisionReward = -0.1f;
//    public float turnRewardFactor = -0.2f;
//    public float forwardRewardFactor = 0.5f;
//    public float distRewardFactor = 0.01f;
//    public float timeRefardFactor = 0.005f;
//    public float energyRewardFactor = 1f;
//    public float powerUpReward = 1f;
//    public float winReward = 1f;
//}

//[System.Serializable]
//public class TankDriverStats
//{
//    public int powerUps = 0;
//    public float totalDistance = 0;

//    public StatsRecorder recorder;
//    public TankDriverStats(StatsRecorder rec)
//    {
//        recorder = rec;
//    }

//    internal void Send()
//    {
//        recorder.Add("driver/PowerUps", powerUps, StatAggregationMethod.Average);
//        recorder.Add("driver/TotalDistance", totalDistance, StatAggregationMethod.Average);

//    }
//}
