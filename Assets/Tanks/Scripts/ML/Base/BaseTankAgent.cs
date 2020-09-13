using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;

public abstract class BaseTankAgent : Agent, ITankAgent
{
    public MLRewards rewards;
    protected bool dead = false;
    protected TanksAreaBase area;
    protected TankController tank;
    protected MLStats stats;
    protected int teamId;
    public float[] actions;

    //For debug
    public float reward;

    public override void Initialize()
    {
        Debug.Log("Initialize");
        area = GetComponentInParent<TanksAreaBase>();
        tank = GetComponent<TankController>();
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

    public virtual void Die()
    {
        dead = true;
        tank.Die();
        EndEpisode();
    }

    private void FixedUpdate()
    {
        if (dead)
            return;
        if (tank.IsDead())
        {
            Die();
            return;
        }
    }





    public virtual void OnEnergyRecharge()
    {
        tank.RechargeEnergy();
        stats.powerUps++;
        PostReward(rewards.agentRewards.powerUpReward);
    }

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

    public virtual void OnHit(DamagableTarget hitObject, float damage)
    {
        throw new NotImplementedException();
    }

    public virtual void OnFire()
    {
        throw new NotImplementedException();
    }

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
      //  sensor.AddObservation(mAgent.teamId == team);
    }

}
