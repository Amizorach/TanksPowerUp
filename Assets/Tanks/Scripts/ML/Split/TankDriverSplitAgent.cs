using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;

public class TankDriverSplitAgent : SplitAgentBase
{
    private string movementAxisName = "Vertical";
    private string turnAxisName = "Horizontal";
   
   
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


    public override void OnEnergyRecharge()
    {
        stats.powerUps++;
        PostReward(rewards.agentRewards.powerUpReward);

    }

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
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        actions = vectorAction;
        if (dead)
            return;
        tank.UpdateMotorInput(vectorAction[0], vectorAction[1]);
    }

    public Vector3 invVel;
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