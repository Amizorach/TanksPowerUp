using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;

public class TankTurretAgent : BaseTankAgent
{
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


    protected  void HandleRewards()
    {
        if (hitTank)
        {
            PostReward(rewards.turretRewards.inScopeRewardFactor * Time.fixedDeltaTime);
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Mathf.RoundToInt(Input.GetAxis(turnTurretAxisName) * 4f) + 5;

        actionsOut[1] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
        
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        actions = vectorAction;
        if (dead)
            return;
        float speed = (vectorAction[0] - 5f) / 4f;
        tank.UpdateTurretInput(speed, vectorAction[1] == 1);
    

    }

    public override void OnHit(DamagableTarget hitObject, float damage)
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
    public bool hitTank = false;

    public override void CollectObservations(VectorSensor sensor)
    {
        RaycastHit hit;
        int layerMask = 1 << 9;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        hitTank = false;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(tank.fireTransform.position, tank.fireTransform.TransformDirection(Vector3.forward), out hit, 50, layerMask))
        {
           if(hit.collider.gameObject.GetComponent<DamagableTank>() != null)
            {
                hitTank = true;
            }
        }
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
