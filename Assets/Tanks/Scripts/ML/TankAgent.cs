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
    public bool hitTank = false;


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

    public override void Die()
    {
        area.ResetAgent(gameObject);
        base.Die();
    }
    //public void OnEnergyRecharge()
    //{
    //    tank.RechargeEnergy();
    //    stats.powerUps++;
    //    PostReward(rewards.agentRewards.powerUpReward);
    //}



    protected void HandleRewards()
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

        if (hitTank)
        {
            PostReward(rewards.turretRewards.inScopeRewardFactor * Time.fixedDeltaTime);
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis(movementAxisName);
        actionsOut[1] = Input.GetAxis(turnAxisName);
        actionsOut[2] = Input.GetAxis(turnTurretAxisName);
        actionsOut[3] = Input.GetAxis(fireAxisName);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        actions = vectorAction;
        if (dead)
            return;
        tank.OnActionReceived(vectorAction[0], vectorAction[1], vectorAction[2], vectorAction[3]);


    }



    public override void OnHit(DamagableTarget hitObject, float damage)
    {

        if (hitObject == null || damage == 0)
        {
            PostReward(rewards.shooterRewards.missReward);
            stats.miss++;
            return;
        }
        stats.hit++;
        stats.totalDamage += damage;
     
        PostReward(rewards.shooterRewards.hitRewardFactor * damage);
        PostReward(rewards.shooterRewards.hitReward);


    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 invVel = this.transform.InverseTransformVector(tank.GetVelocity());
        sensor.AddObservation(invVel); // vec 3
        sensor.AddObservation(tank.energy);

        //foreach (YTFlag flag in area.flags)
        //{
        //    AddTeamedObservation(sensor, flag.transform, flag.teamId);
        //}
        RaycastHit hit;
        int layerMask = ~(1 << 9);
        hitTank = false;
        if (Physics.Raycast(tank.fireTransform.position, tank.fireTransform.TransformDirection(Vector3.forward), out hit, 50, layerMask))
        {

            if (hit.collider.gameObject.GetComponent<DamagableTank>() != null)
            {
                Debug.DrawRay(tank.fireTransform.position, tank.fireTransform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

                hitTank = true;
            }
        }
        sensor.AddObservation(hitTank);

        HandleRewards();
    }


    public override void OnFire()
    {
        stats.shots++;
        if (hitTank)
            PostReward(rewards.shooterRewards.hitReward);

    }


}
