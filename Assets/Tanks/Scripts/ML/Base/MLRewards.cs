using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class MLRewards {
    public AgentRewards agentRewards;
    public DriverRewards driverRewards;
    public TurretRewards turretRewards;
    public ShooterRewards shooterRewards;

}


[System.Serializable]

public class AgentRewards
{
    public float timeRewardFactor = -0.01f;
    public float energyRewardFactor = 0.01f;
    public float powerUpReward = 2f;
    public float remainingAgetsRewardFactor = 0f;
    public float positionRewardFactor = 5f;

    public float damageRewardFactor = -3f;
    public float winReward = 1F;
}
[System.Serializable]
public class DriverRewards
{
    public float collisionReward = -0.1f;
    public float turnRewardFactor = -0.2f;
    public float forwardRewardFactor = 0.5f;
    public float distRewardFactor = 0.01f;
}

[System.Serializable]
public class TurretRewards
{
    public float turnRewardFactor = 0.05f;
    public float inScopeRewardFactor = 0.1f;
}

[System.Serializable]
public class ShooterRewards
{
    public float hitRewardFactor = 1f;
    public float hitReward = 1f;
    public float missReward = -0.1f;
    public float inactiveReward = -0.05f;
    public float freindyHitRewardFactor = -1f;
}
