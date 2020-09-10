using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;

public class TankAgent : BaseTankAgent
{
    private string movementAxisName = "Vertical";
    private string turnAxisName = "Horizontal";
    private string turnTurretAxisName = "Turret";
    private string fireAxisName = "Fire";


    public int counter = 1;

    public void UpdateModel()
    {
        counter++;
        if (counter >= 8)
            counter = 1;
        SetModel("YTDriver" + counter, null);
    }

  
    public void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.tag == "wall")
        {
            PostReward(rewards.driverRewards.collisionReward * Time.fixedDeltaTime);
        }
    }

    //public void OnEnergyRecharge()
    //{
    //    tank.RechargeEnergy();
    //    stats.powerUps++;
    //    PostReward(rewards.agentRewards.powerUpReward);
    //}



    protected override void HandleRewards()
    {

        //time
        PostReward(rewards.agentRewards.timeRewardFactor);

        //Energy
        PostReward(tank.energy * rewards.agentRewards.energyRewardFactor * Time.fixedDeltaTime);

        //Angle change
        float aAng = Mathf.Abs(tank.GetTurnInput());
        if (aAng > 10f)
            PostReward(rewards.driverRewards.turnRewardFactor * (aAng / 180f) * Time.fixedDeltaTime);

        //Speed
        if (tank.GetSpeed() > 0)
            PostReward(rewards.driverRewards.forwardRewardFactor * tank.GetSpeed() * Time.fixedDeltaTime);

        //Distance
        PostReward(rewards.driverRewards.distRewardFactor * tank.GetSpeed() * Time.fixedDeltaTime);

    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis(movementAxisName);
        actionsOut[1] = Input.GetAxis(turnAxisName);
        actionsOut[2] = Input.GetAxis(turnTurretAxisName);
        actionsOut[3] = Input.GetAxis(fireAxisName);
    }

    //public override void OnActionReceived(float[] vectorAction)
    //{
    //    actions = vectorAction;
    //    if (dead)
    //        return;
    //    tank.OnActionReceived(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);

    //}

    public Vector3 invVel;

    internal void OnHit(DamagableTarget hitObject, float damage)
    {
        stats.shots++;

        if (hitObject == null || damage == 0)
        {
            PostReward(rewards.shooterRewards.missReward);
            stats.miss++;
            return;
        }
        stats.hit++;
        stats.totalDamage += damage;

        //if (hitObject.GetComponent<TankAgent>().teamId == teamId)
        //{
        //    PostReward(rewards.shooterRewards.freindyHitRewardFactor * damage);

        //}

        PostReward(rewards.shooterRewards.hitRewardFactor * damage);
        PostReward(rewards.shooterRewards.hitReward);


    }

    public override void CollectObservations(VectorSensor sensor)
    {
        invVel = this.transform.InverseTransformVector(tank.GetVelocity());
        sensor.AddObservation(invVel); // vec 3
        sensor.AddObservation(tank.energy);

        //foreach (YTFlag flag in area.flags)
        //{
        //    AddTeamedObservation(sensor, flag.transform, flag.teamId);
        //}

        HandleRewards();
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
