using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System;
using Unity.Barracuda;

public class TankDriverAgent : Agent
{
    private string movementAxisName = "Vertical";
    private string turnAxisName = "Horizontal";

    private bool dead = false;
    private TanksAreaBase area;
    private TankController tank;
    private TankDriverStats stats; //stats used to send to TensorBoard
    public TankDriverRewards driverRewards; // reward values 


    //For debug
    [HeaderAttribute("Debug")]
    public float reward;

    public override void Initialize()
    {
        Debug.Log("Initialize");
        area = GetComponentInParent<TanksAreaBase> ();
        tank = GetComponent<TankController>();
    }

    public void SetModelName(String name)
    {

        GetComponent<BehaviorParameters>().BehaviorName = name;
    }
    internal void SetModel(NNModel model)
    {
        GetComponent<BehaviorParameters>().Model = model;
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");

        //handle the last episodes stats ///EndEpisode will not always get called
        if (stats != null)
            SendStats();
        reward = 0f;
        //if (area.GetDriverRewards() != null)
            //driverRewards = area.GetDriverRewards();
        area.ResetAgent(this.gameObject);

        tank.OnEpisodeBegin(null);
        stats = new TankDriverStats(Academy.Instance.StatsRecorder);

        dead = false;
    }

    internal void UpdateSettings(TankSettings tankSettings)
    {
        tank.UpdateSettings(tankSettings);
    }

    internal void UpdateRewards(TankDriverRewards driverRewards)
    {
        this.driverRewards = driverRewards;
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.tag == "wall")
        {
            PostReward(driverRewards.collisionReward * Time.fixedDeltaTime);
        }
    }

    public void OnEnergyRecharge()
    {
        tank.RechargeEnergy();
        stats.powerUps++;
        PostReward(driverRewards.powerUpReward);
    }


    private void Win()
    {
        PostReward(driverRewards.winReward);
        Die();
    }

    private void Die()
    {
        dead = true;
        tank.Die();
        EndEpisode();
    }

    private void FixedUpdate()
    {
        if (dead)
            return;
        //if (CheckForEndGame())
        //    return;

        if (tank.IsDead())
        {
            Die();
            return;
        }
    }

    private void HandleDriverRewards()
    {
      
        //time
        PostReward(driverRewards.timeRewardFactor);

        //Energy
        PostReward(tank.energy * driverRewards.energyRewardFactor * Time.fixedDeltaTime);

        //Angle change
        float aAng = Mathf.Abs(tank.GetTurnInput());
        if (aAng > 10f)
            PostReward(driverRewards.turnRewardFactor * (aAng / 180f) * Time.fixedDeltaTime);

        //Speed
        if (tank.GetSpeed() > 0)
           PostReward(driverRewards.forwardRewardFactor * tank.GetSpeed() * Time.fixedDeltaTime);

        //Distance
        PostReward(driverRewards.distRewardFactor * tank.GetSpeed() * Time.fixedDeltaTime);

    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis(movementAxisName);
        actionsOut[1] = Input.GetAxis(turnAxisName);
    }

    public float[] actions;
    public override void OnActionReceived(float[] vectorAction)
    {
        actions = vectorAction;
        if (dead)
            return;
        tank.OnActionReceived(vectorAction[0], vectorAction[1]);
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

        HandleDriverRewards();
    }
    
    //public void AddObservationForTransform(VectorSensor sensor, Transform target)
    //{
    //    //4 for flag
    //    Vector3 dirToTarget = (target.position - transform.position).normalized;
    //    sensor.AddObservation(
    //           transform.InverseTransformDirection(dirToTarget)); // vec 3
    //    Vector3 invPos = transform.InverseTransformPoint(transform.position);
    //    sensor.AddObservation(invPos.x); // vec 3
    //    sensor.AddObservation(invPos.z); // vec 3
    //}

    //public void AddTeamedObservation(VectorSensor sensor, Transform target, int team)
    //{
    //    AddObservationForTransform(sensor, target);
    //    sensor.AddObservation(teamId == team);
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

   
}
[System.Serializable]

public class TankDriverRewards
{
    public float collisionReward = -0.1f;
    public float turnRewardFactor = -0.2f;
    public float forwardRewardFactor = 0.5f;
    public float distRewardFactor = 0.01f;
    public float timeRewardFactor = 0.005f;
    public float energyRewardFactor = 1f;
    public float powerUpReward = 1f;
    public float winReward = 1f;
}

[System.Serializable]
public class TankDriverStats
{
    public int powerUps = 0;
    public float totalDistance = 0;

    public StatsRecorder recorder;
    public TankDriverStats(StatsRecorder rec)
    {
        recorder = rec;
    }

    internal void Send()
    {
        recorder.Add("driver/PowerUps", powerUps, StatAggregationMethod.Average);
        recorder.Add("driver/TotalDistance", totalDistance, StatAggregationMethod.Average);

    }
}