using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class MultiBrainAgent : MonoBehaviour, ITankAgent
{
    public SplitAgentBase[] agents;
    public TankController tank;
    private MLRewards rewards;
    public TanksAreaBase area;
     

    public int teamId;
    public int playerId;
    public bool dead;

    public GameObject brainHolder;

    public void Awake()
    {
        tank = GetComponent<TankController>();

        agents = GetComponentsInChildren<SplitAgentBase>();
        area = GetComponentInParent<TanksAreaBase>();
        Reset();
       // EnableAgents(false);
        //foreach (BaseTankAgent agent in agents)
        //{
        //    agent.masterBrain = this;
        //    agent.ConfigSetup(settings.agentSettings);
        //    agent.SetPlayerId(playerId, getRandomTankColor());
        //    FindEmptySpace(go);
        //}
    }

    public void EnableAgents(bool enable)
    {
        foreach (SplitAgentBase agent in agents)
        {
            agent.gameObject.SetActive(enable);
        }

    }

    private void FixedUpdate()
    {
        if (dead)
        {
            Reset();
            return;
        }
        if (tank.IsDead())
        {
            Die();
            return;
        }
    }

    //public void ConfigSetup(int tId, AgentSettings agentSettings)
    //{
    //    settings = agentSettings;
    //    teamId = tId;
    //    tank.ConfigSetup(settings.tankSettings);
    //    driver.ConfigSetup(teamId, this);
    //    shooter.ConfigSetup(teamId, this);

    //}
    public void Reset()
    {
        dead = false;
        gameObject.SetActive(true);
    }

    //public void Reset(TeamInfo tInfo, int playerId, MultiAgentSpawnInfo info)
    //{
    //    Debug.Log("reset " + playerId);
    //    this.playerId = playerId;
    //    this.teamId = tInfo.teamId;

    //    driverEnabled = info.driverEnabled;
    //    shooterEnabled = info.shooterEnabled;

    //    driver.gameObject.SetActive(false);
    //    shooter.gameObject.SetActive(false);
    //    agentRewards = info.agentRewards;

    //    tank.SetBaseColor(tInfo.baseColor);
    //    tank.ConfigSetup(info.tankSettings);
    //    tank.ResetColor();
    //    if (driverEnabled)
    //    {
    //        DriverReset(info.driverModel, info.driverRewards);
    //        driver.gameObject.SetActive(true);
    //        // driver.enabled = true;
    //    }
    //    if (shooterEnabled)
    //    {
    //        ShooterReset(info.shooterModel, info.ShooterRewards);
    //        //shooter.enabled = false;
    //        shooter.gameObject.SetActive(true);

    //    }

    //}


    //private void DriverReset(BehaviorModelSettings driverModel, DriverRewards driverRewards)
    //{
    //    driverModel.InitAgent(teamId, driver.behaviorParameters);
    //    driver.driverRewards = driverRewards;
    //}
    //private void ShooterReset(BehaviorModelSettings shooterModel, ShooterRewards shooterRewards)
    //{
    //    shooterModel.InitAgent(teamId, shooter.behaviorParameters);
    //    shooter.shooterRewards = shooterRewards;
    //}

    public void PostReward(float reward)
    {
        foreach (SplitAgentBase agent in agents)
        {
            agent.PostReward(reward);
        }
    }


    //public void TakeDamage(float damage)
    //{
    //    tank.TakeDamage(damage);
    //    PostReward(settings.rewardSettings.agentRewards.damageRewardFactor * damage);
    //}

    

    public void Die()
    {
        Debug.Log("die");
        dead = true;
        gameObject.SetActive(false);
        foreach (SplitAgentBase agent in agents)
        {
            agent.Die();
        }
        area.ResetAgent(gameObject);
    }

    public void Win()
    {
        Debug.Log("win");
        dead = true;
        foreach (SplitAgentBase agent in agents)
        {
            agent.Win();
            agent.Die();
        }

    }

    public bool isDead()
    {
        if (dead)
            return true;
        if (tank.dead)
        {
            Die();
            return true;
        }
        return false;
    }

    public static Color getRandomTankColor()
    {
        return UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

    }

  
    internal void EndEpisode()
    {
        foreach (SplitAgentBase agent in agents)
        {
            agent.EndEpisode();
        }
    }


    public void OnEnergyRecharge()
    {
        tank.RechargeEnergy();
        foreach (SplitAgentBase agent in agents)
        {
            agent.OnEnergyRecharge();
        }
      
    }

    public void OnHit(DamagableTarget hitObject, float damage)
    {

        foreach (SplitAgentBase agent in agents)
        {
            agent.OnHit(hitObject, damage);
        }

    }

    public void OnFire()
    {
        foreach (SplitAgentBase agent in agents)
        {
            agent.OnFire();
        }
    }
}
